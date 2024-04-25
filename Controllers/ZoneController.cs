using Microsoft.AspNetCore.Mvc;
using sl_APIs.Controller.Controller;
using UWEServer.Data;
using UWEServer.Entities;
using UWEServer.Responses;
using UWEServer.Requests;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using UWEServer.Model;
using Newtonsoft.Json;
using UWEServer.Services;
using Microsoft.EntityFrameworkCore;

namespace UWEServer.Controllers
{
    [Route("api/zone")]
    [ApiController]
    public class ZoneController : APIBaseController
    {
        #region Private variables
        private readonly DbApiContext _dbApiContext;
        #endregion

        #region Public variables

        #endregion

        #region Contructor
        public ZoneController(DbApiContext dbApiContext)
        {
            _dbApiContext = dbApiContext;
        }


        [Route("create")]
        [HttpPost]
        public ApiJsonResult CreateZone([FromBody] ZoneTransfer zoneCreate)
        {
            try
            {
                //zoneCreate = ConverService.ConvertValueInput(zoneCreate);
                if (string.IsNullOrEmpty(zoneCreate.Name))
                {
                    return new ApiJsonResult()
                    {
                        code = 422,
                        message = "Zone name is null"
                    };
                }
                if (zoneCreate.Height <= 0 || zoneCreate.Width <= 0)
                {
                    return new ApiJsonResult()
                    {
                        code = 423,
                        message = "Zone height or width is null"
                    };

                }
                //Check duplicate name
                var zoneCheck = _dbApiContext.Zones.FirstOrDefault(x => x.Name == zoneCreate.Name && !x.DelFlag);
                if (zoneCheck != null)
                {
                    return new ApiJsonResult()
                    {
                        code = 424,
                        message = "Zone Name already exists"
                    };
                }
                var zoneID = Guid.NewGuid().ToString().Replace("-", "").Remove(0, 12);
                var newZone = new Zone()
                {
                    ZoneId = zoneID,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now, 
                    DelFlag = false,
                    Name = zoneCreate.Name,    
                    Width = zoneCreate.Width,
                    Height = zoneCreate.Height
                };
                _dbApiContext.Add(newZone);
                _dbApiContext.SaveChanges();
                var terminalId = Guid.NewGuid().ToString().Replace("-", "").Remove(0, 12);
                var newTerminal = new Terminal()
                {
                    Name = "Default Terminal",
                    TerminalId = terminalId,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    DelFlag = false,
                    Zone = newZone,
                    ZoneId = newZone.Id,
                    Height = newZone.Height/10,
                    Width = newZone.Width / 10,
                    Front = 0,
                    Latitude = 0,
                    Longtitude = 0,
                };
                _dbApiContext.Add(newTerminal);
                _dbApiContext.SaveChanges();

                //_cacheService.AddCache(new UpdateCacheDataRequest()
                //{
                //    Data = newZone
                //});
                return new ApiJsonResult()
                {
                    code = 200,
                    message = "Ok",
                    data = new ZoneDetailsResponse()
                    {
                        Id = newZone.Id,
                        ZoneID = newZone.ZoneId,
                        Name = newZone.Name,
                        Height = newZone.Height,
                        Width = newZone.Width,
                        CreatedTime = newZone.CreatedTime,
                        UpdatedTime = newZone.UpdatedTime
                      
                    }
                };
            }
            catch (Exception e)
            {
                Log.Error(e.ToString(), "CreateZone fail");
                return APIException(e);
            }

        }


        /// <summary>
        /// Get all zones
        /// </summary>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        public ApiJsonResult GetZones([FromQuery] PaginationFilter queryData)
        {
            {
                try
                {
                    var zoneQuery = _dbApiContext.Zones.Where(m => !m.DelFlag);
                    var route = Request.Path.Value;
                    var filter = PaginationHelperResponse.GetDataFilter(queryData.Filter);
                    var sorts = string.IsNullOrEmpty(queryData.Sort) ? new List<DataSort>() :
                        JsonConvert.DeserializeObject<List<DataSort>>(queryData.Sort);
                    zoneQuery = PaginationHelperResponse.QueryInTable<Zone>(zoneQuery, filter, sorts);
                    zoneQuery = zoneQuery.OrderBy(m => m.Id);
                    var pagedResponse = PaginationHelperResponse.CreatePagedReponse<Zone>(zoneQuery, queryData, 0, route);
                    return new ApiJsonResult()
                    {
                        code = 200,
                        message = "Ok",
                        data = new
                        {
                            zone = pagedResponse.Data.Select(m => ConvertService.ConvertZoneToBasicData(m)).ToList(),
                            meta = new
                            {
                                pagedResponse.PageNumber,
                                pagedResponse.PageSize,
                                pagedResponse.TotalPages,
                                pagedResponse.TotalRecords,
                                pagedResponse.Succeeded,
                                pagedResponse.Errors,
                                pagedResponse.Message,

                            }
                        }
                    };
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString(), "GetZones failed");
                    return APIException(e);
                }
            }

        }


        /// <summary>
        /// Delete Zone
        /// </summary>
        /// <returns></returns>
        [Route("delete")]
        [HttpDelete]
        public ApiJsonResult DeleteZone(int id)
        {
            try
            {
                var zone = _dbApiContext.Zones.Include(m => m.Terminals).Include(m => m.Blocks).FirstOrDefault(m => m.Id == id && !m.DelFlag);
                if (zone == null)
                {
                    return new ApiJsonResult()
                    {
                        code = 400,
                        message = " Zone not found"
                    };
                }

                // Delele every terminal info one by one
                var now = DateTime.Now;                
                foreach (var block in zone.Blocks)
                {
                    block.UpdatedTime = now;
                    block.DelFlag = true;
                    _dbApiContext.Update(block);
                }
                foreach (var terminal in zone.Terminals)
                {
                    terminal.DelFlag = true;
                    terminal.UpdatedTime = now;
                    _dbApiContext.Update(terminal);
                }

                zone.DelFlag = true;
                zone.UpdatedTime = DateTime.Now;
                _dbApiContext.Update(zone);
                _dbApiContext.SaveChanges();           
                //_cacheService.AddCache(new UpdateCacheDataRequest()
                //{
                //    Data = zone
                //});
                return new ApiJsonResult()
                {
                    code = 200,
                    message = "Zone deleted successfully"
                };
            }
            catch (Exception e)
            {
                Log.Error(e.ToString(), "DeleteZone failed");
                return APIException(e);
            }
        }

        /// <summary>
        /// Get zone details
        /// </summary>
        /// <returns></returns>
        [Route("get-zone-details")]
        [HttpGet]
        public ApiJsonResult GetZoneDetails(int id)
        {
            {
                try
                {
                    var zone = _dbApiContext.Zones.Include(m=> m.Terminals).Include(m=>m.Blocks).FirstOrDefault(m => !m.DelFlag && m.Id == id);
                    if (zone == null)
                    {
                        return new ApiJsonResult()
                        {
                            code = 422,
                            message = "No Zone found "
                        };
                    }
                    //var terminals = _dbApiContext.Terminals.Where(m => !m.DelFlag && !m.Zone.DelFlag && m.ZoneId == zoneId);
                    //var blocks = _dbApiContext.Blocks.Where(m => !m.DelFlag && !m.Zone.DelFlag && m.ZoneId == zoneId);
                    var terminals = zone.Terminals.Where(m => !m.DelFlag);
                    var blocks = zone.Blocks.Where(m => !m.DelFlag);


                    return new ApiJsonResult()
                    {
                        code = 200,
                        message = "Ok",
                        data = new
                        {
                            zone = new
                            {
                                id = zone.Id,
                                zone_id = zone.ZoneId,
                                name = zone.Name,
                                url_bg = zone.ImageUrl,
                                width = zone.Width,
                                height = zone.Height,
                                block_count = blocks.Count(),
                                terminal_count = terminals.Count(),
                            },
                            terminals = terminals.Select(m => new {
                                id = m.Id,
                                terminal_id = m.TerminalId,
                                name = m.Name,
                                width = m.Width,
                                height = m.Height,
                                lng = m.Longtitude,
                                lat = m.Latitude,
                                front = m.Front,
                                color = m.Color,
                            }).ToList(),
                            blocks = blocks.Select(m => new {
                                id = m.Id,
                                block_id = m.BlockId,
                                name = m.Name,
                                width = m.Width,
                                height = m.Height,
                                lng = m.Longtitude,
                                lat = m.Latitude,
                                front = m.Front,
                                color = m.Color,
                                category = m.CategoryId != null ? m.CategoryId : 0,
                                zoneId = m.ZoneLinkId != null ? m.ZoneLinkId : 0
                            }).ToList(),
                        }
                    };
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString(), "UpdateZone fail");
                    return APIException(e);
                }
            }
        }

        /// <summary>
        /// Get zone details
        /// </summary>
        /// <returns></returns>
        [Route("kiosk/get-zone-details")]
        [HttpGet]
        public ApiJsonResult KioskGetZoneDetails(string zoneId)
        {
            {
                try
                {
                    var zone = _dbApiContext.Zones.Include(m => m.Terminals).Include(m => m.Blocks).ThenInclude(m=>m.Category).FirstOrDefault(m => !m.DelFlag && m.ZoneId == zoneId);
                    if (zone == null)
                    {
                        return new ApiJsonResult()
                        {
                            code = 422,
                            message = "No Zone found "
                        };
                    }
                    var zoneResponse = new ZoneKioskResponse();
                    zoneResponse.ZoneId = zoneId;
                    zoneResponse.ImgUrl = zone.ImageUrl != null ? zone.ImageUrl : "";
                    zoneResponse.Id = zone.Id;
                    zoneResponse.Name = zone.Name;
                    zoneResponse.Width = zone.Width;
                    zoneResponse.Height = zone.Height;
                    zoneResponse.ObjectList = new List<Responses.ZoneObject>();
                    //var terminals = _dbApiContext.Terminals.Where(m => !m.DelFlag && !m.Zone.DelFlag && m.ZoneId == zoneId);
                    //var blocks = _dbApiContext.Blocks.Where(m => !m.DelFlag && !m.Zone.DelFlag && m.ZoneId == zoneId);
                    var terminals = zone.Terminals.Where(m => !m.DelFlag);
                    var blocks = zone.Blocks.Where(m => !m.DelFlag);
                    foreach (var item in terminals)
                    {
                        var terminal = new Responses.ZoneObject()
                        {
                            Id = item.Id,
                            Type = 1,
                            ObjectId = item.TerminalId,
                            Name = item.Name,
                            Width = item.Width,
                            Height = item.Height,
                            Front = item.Front,
                            Lat = item.Latitude,
                            Lng = item.Longtitude,
                            Color = item.Color != null? item.Color : "",
                            Category = "",
                        };
                        zoneResponse.ObjectList.Add(terminal);  
                    }

                    foreach (var item in blocks)
                    {
                        var zoneLinkName = "";
                        if(item.ZoneLinkId != 0)
                        {
                            var zoneLink = _dbApiContext.Zones.FirstOrDefault(m => !m.DelFlag && m.Id == item.ZoneLinkId);
                            zoneLinkName = zoneLink?.Name;
                        }
                        var block = new Responses.ZoneObject()
                        {
                            Id = item.Id,
                            Type = 2,
                            ObjectId = item.BlockId,
                            Name = item.ZoneLinkId == 0 ? item.Name : zoneLinkName,
                            Width = item.Width,
                            Height = item.Height,
                            Front = item.Front,
                            Lat = item.Latitude,
                            Lng = item.Longtitude,
                            Color = item.Color != null ? item.Color : "",
                            Category = item.Category?.Name != null ? item.Category.Name : "Zone",
                        };
                        zoneResponse.ObjectList.Add(block);
                    }

                    return new ApiJsonResult()
                    {
                        code = 200,
                        message = "Ok",
                        data = zoneResponse
                    };
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString(), "ZoneKioskInfo fail");
                    return APIException(e);
                }
            }
        }

        /// <summary>
        /// Update draw zone
        /// </summary>
        /// <returns></returns>
        [Route("update-draw-zone")]
        [HttpPost]
        public ApiJsonResult UpdateDrawZone(ZoneDraw updateZone)
        {
            {
                try
                {
                    var zoneCheck = _dbApiContext.Zones.Include(m => m.Blocks).Include(m => m.Terminals).FirstOrDefault(m => !m.DelFlag && m.Id == updateZone.Id);
                    if (zoneCheck == null)
                    {
                        return new ApiJsonResult()
                        {
                            code = 422,
                            message = "No Zone found"
                        };

                    }
                    var terminals = zoneCheck.Terminals.Where(m => !m.DelFlag);
                    var blocks = zoneCheck.Blocks.Where(m => !m.DelFlag).ToList();
                    var temp = updateZone.ObjectList.Where(m => m.Type == 2 && m.Id != 0);
                    var deletedBlocks = blocks.Where(m => temp.All(n => n.Id != m.Id)); // Find deleted blocks
                    var newBlocks = updateZone.ObjectList.Where(m => m.Type == 2 && m.Id == 0); // Find newly created blocks
                    foreach (var deletedBlock in deletedBlocks)
                    {
                        deletedBlock.UpdatedTime = DateTime.Now;
                        deletedBlock.DelFlag = true;
                        _dbApiContext.Update(deletedBlock);
                    }
                    foreach (var newBlock  in newBlocks)
                    {
                        //var blockID = Guid.NewGuid().ToString().Replace("-", "").Remove(0, 12);
                        var block = new Block()
                        {
                            Name = newBlock.Name,
                            Latitude = newBlock.Lat,
                            Longtitude = newBlock.Lng,
                            Color = newBlock.Color,
                            Height = newBlock.Height,
                            Width = newBlock.Width,
                            Front = newBlock.Front,
                            UpdatedTime = DateTime.Now,
                            CategoryId = newBlock.CategoryId != 0 ? newBlock.CategoryId : null,
                            ZoneLinkId = newBlock.ZoneLinkId,
                            BlockId = newBlock.ObjectId,
                            CreatedTime = DateTime.Now,
                            DelFlag = false,
                            //Zone = zoneCheck,
                            ZoneId = zoneCheck.Id,
                        };
                        _dbApiContext.Add(block);

                    }
                    foreach (var ob in updateZone.ObjectList.Where(m=> m.Id != 0)) // Update old blocks and terminals
                    {
                        if (ob.Type == 1) // if object is terminal 
                        {
                            foreach (var terminal in terminals)
                            {
                                if (ob.Id == terminal.Id)
                                {
                                    terminal.Latitude = ob.Lat;
                                    terminal.Longtitude = ob.Lng;
                                    terminal.Height = ob.Height;
                                    terminal.Width = ob.Width;
                                    terminal.Front = ob.Front;
                                    terminal.Color = ob.Color;
                                    terminal.UpdatedTime = DateTime.Now;
                                    _dbApiContext.Update(terminal);
                                    break;
                                }
                            }

                        }
                        if (ob.Type == 2 ) // if object is block 
                        {
                            foreach (var block in blocks)
                            {
                                if (ob.Id == block.Id)
                                {
                                    block.Name = ob.Name;
                                    block.Latitude = ob.Lat;
                                    block.Longtitude = ob.Lng;
                                    block.Height = ob.Height;
                                    block.Width = ob.Width;
                                    block.Front = ob.Front;
                                    block.Color = ob.Color;
                                    block.CategoryId = ob.CategoryId != 0 ? ob.CategoryId : null ;
                                    block.ZoneLinkId = ob.ZoneLinkId;
                                    block.UpdatedTime = DateTime.Now;
                                    _dbApiContext.Update(block);
                                    break;
                                }
                            }
                        }
                    }
                    zoneCheck.Width = updateZone.Width;
                    zoneCheck.Height = updateZone.Height;
                    _dbApiContext.Update(zoneCheck);
                    _dbApiContext.SaveChanges();
                    return new ApiJsonResult()
                    {
                        code = 200,
                        message = "Ok"
                    };
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString(), "UpdateDrawZone fail");
                    return APIException(e);
                }
            }
        }


        /// <summary>
        /// Update background
        /// </summary>
        /// <returns></returns>
        [Route("update/background")]
        [HttpPost]
        public async Task<ApiJsonResult> UpdateBG(IFormFile file, int id)
        {
            try
            {
                var zone = _dbApiContext.Zones.FirstOrDefault(m => m.Id == id && !m.DelFlag);
                if (zone == null)
                {
                    return new ApiJsonResult()
                    {
                        code = 422,
                        message = $"No Zone found"
                    };
                }
                else
                {
                    var checkFileName = file.FileName.Split(new char[] { '.' });
                    var lastFileName = checkFileName[checkFileName.Length - 1].ToUpper();
                    if (lastFileName != "JPG" && lastFileName != "PNG" && lastFileName != "JPEG")
                    {
                        return new ApiJsonResult()
                        {
                            code = 422,
                            message = $"Not support file {lastFileName}"
                        };
                    }
                    string path = $"{Directory.GetCurrentDirectory()}/wwwroot/Uploads";
                    var result = await ConvertService.SaveFile(file, path, "Zone", zone.Name);
                   zone.ImageUrl = result;
                    _dbApiContext.Update(zone);
                    _dbApiContext.SaveChanges();
                    return new ApiJsonResult()
                    {
                        code = 200,
                        message = "Update image background successfully",
                        data = zone.ImageUrl
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString(), "Update image background failed");
                return APIException(e);
            }
        }

        /// <summary>
        /// Get zone enum
        /// </summary>
        /// <returns></returns>
        [Route("enum")]
        [HttpGet]
        public ApiJsonResult GetZoneEnum(int id)
        {
            {
                try
                {
                    var listZones = _dbApiContext.Zones.Where(m => m.DelFlag == false && m.Id != id).ToList().Select(m =>
                    new { id = m.Id, name = m.Name }).ToList();
                    return new ApiJsonResult()
                    {
                        code = 200,
                        message = "OK",
                        data = listZones
                    };
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString(), "GetZoneEnum failed");
                    return APIException(e);
                }
            }

        }

        #endregion
    }
}
