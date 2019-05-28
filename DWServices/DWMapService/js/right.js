$(function () {
    selectL = $(".station").find(".selected").prev().html();
    selectB = $(".type").find(".selected").prev().html();
    require([
        "esri/config",
        "esri/Basemap",
        "esri/layers/TileLayer",
        "esri/layers/WMSLayer",
        "esri/layers/FeatureLayer",
        "esri/Map",
        "esri/views/MapView",
        "esri/Graphic",
        "dojo/domReady!"
    ], function (esriConfig, Basemap, TileLayer, WMSLayer, FeatureLayer, Map, MapView, Graphic) {
        window.WMSLayer = WMSLayer;
        window.FeatureLayer = FeatureLayer;
        window.Graphic = Graphic;
        esriConfig.request.corsEnabledServers.push(
            "192.168.1.104:3397");
        if (token) {
            var layer = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/shijiazhuangMap/MapServer" + (token ? "?token=" + token : "")

            });
            var layer2 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/baodingMap/MapServer" + (token ? "?token=" + token : "")

            });
            var layer3 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/cangzhouMap/MapServer" + (token ? "?token=" + token : "")

            });
            var layer4 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/handanMap/MapServer" + (token ? "?token=" + token : "")

            });
            var layer5 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/langfangMap/MapServer" + (token ? "?token=" + token : "")

            });
            var layer6 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/tangshanMap/MapServer" + (token ? "?token=" + token : "")
            });
            var layer7 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/hengshuiMap/MapServer" + (token ? "?token=" + token : "")
            });
            var layer8 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/qinhuangdaoMap/MapServer" + (token ? "?token=" + token : "")
            });
            var layer9 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/zhangjiakouMap/MapServer" + (token ? "?token=" + token : "")
            });
            var layer10 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/chengdeMap/MapServer" + (token ? "?token=" + token : "")
            });
            var layer11 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/xionganMap/MapServer" + (token ? "?token=" + token : "")
            });
            var layer12 = new TileLayer({
                url: "http://136.142.1.144:8090/arcgis/rest/services/xingtaiMap/MapServer" + (token ? "?token=" + token : "")
            });
            mainLayer = new WMSLayer({
                url: "/map/get",
                sublayers: [{
                    name: selectL + "_" + selectB
                }]
            });
            var customBasemap = new Basemap({
                baseLayers: [layer, layer2, layer3, layer4, layer5, layer6, layer7, layer8, layer9, layer10, layer11, layer12],
                title: "Custom Basemap",
                id: "myBasemap"
            });
            myMap = new Map({
                basemap: customBasemap
            });
        } else {
            var layer = new TileLayer({
                //url: "http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"
                url: "http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"
            });
            var layer2 = new TileLayer({
                //url: "http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"
                url: "http://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer"
            });
            var layer3 = new TileLayer({
                //url: "http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"
                url: "http://services.arcgisonline.com/ArcGIS/rest/services/Reference/World_Boundaries_and_Places/MapServer"
            });
            var layer4 = new WMSLayer({
                //url: "http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"
                url: "http://192.168.1.103:3374",
                sublayers: [{
                    name: "lte_flow"
                }]
            });
            mainLayer = new WMSLayer({
                url: "/map/get",
                sublayers: [{
                    name: selectL + "_" + selectB
                }]
            });
            var customBasemap = new Basemap({
                baseLayers: [layer, layer2, layer3, layer4],
                title: "Custom Basemap",
                id: "myBasemap"
            });
            myMap = new Map({
                basemap: customBasemap
            });
        }
        myMap.add(mainLayer);
        view = new MapView({
            center: [116.3836111, 39.9075], // long, lat
            container: "viewDiv",
            map: myMap,
            zoom: minzoom,
            extent: layer.fullExtent
        });
        rollMap();
    });
    switchImg();
    query();
});

var selectL, selectB, myMap, view, mainLayer;


function switchImg() {
    $(".right img").click(function () {
        var path = $(this).attr("src");
        var name = path.split("/")[2];
        var openOrClose = name.split(".")[0].split("_")[1];
        if (openOrClose === "open") return false;
        var parent = $(this).parent().parent().attr("class");
        if (parent.indexOf("station") >= 0) {
            parent = "station";
        } else {
            parent = "type";
        }
        var imgs = $("." + parent).find("img");
        for (var i = 0; i < imgs.length; i++) {
            var imgname = $(imgs[i]).attr("src").split("/")[2].split("_")[0];
            $(imgs[i]).attr("src", "/" + path.split("/")[1] + "/" + imgname + "_close.png");
            $(imgs[i]).removeClass("selected");
        }
        $(this).addClass("selected");
        $(this).attr("src", "/" + path.split("/")[1] + "/" + name.split("_")[0] + "_open.png");
        selectL = $(".station").find(".selected").prev().html();
        selectB = $(".type").find(".selected").prev().html();
        $(".fazhi").hide();
        if (selectL == "NB-IoT") {
            $(".type").find("img").each(function () {
                //$(this).removeClass("selected");

                //$(".type").find("img").eq(3).addClass("selected");
                var picn = $(this).attr("src");
                if (picn.indexOf("cover_") > 0) {
                    $(this).addClass("selected");
                    $(this).attr("src", picn.replace("_close", "_open"));
                } else {
                    $(this).removeClass("selected");
                    $(this).attr("src", picn.replace("_open", "_close"));
                }
            });
            $("#nball").show();
        }
        else if (selectL != "CDMA") {
            if (selectB == "流量") {
                if (selectL == "L800M") {
                    $("#ll800").show();
                }
                else if (selectL == "L1.8G") {
                    $("#ll1800").show();
                }
                else if (selectL == "L2.1G") {
                    $("#ll2100").show();
                }
                else if (selectL == "L2.6G") {
                    $("#ll2600").show();
                }
            }
            else if (selectB == "PRB利用率") {
                $("#prball").show();
            }
            else if (selectB == "RRC连接数") {
                $("#rrcall").show();
            }
            else if (selectB == "覆盖") {
                $("#fgnonb").show();
            }
        }
        if (selectL == "CDMA") {
            $("#ENBIDl").html("BTSNUM");
            $("#enbid").attr("placeholder", "BTSNUM");
            $("#CELLIDl").html("CELLNUM");
            $("#cellid").attr("placeholder", "CELLNUM");
        } else {
            $("#ENBIDl").html("ENBID");
            $("#enbid").attr("placeholder", "ENBID");
            $("#CELLIDl").html("CELLID");
            $("#cellid").attr("placeholder", "CELLID");
        }

        myMap.remove(mainLayer);

        mainLayer = new WMSLayer({
            url: "/map/get",
            sublayers: [{
                name: selectL + "_" + selectB
            }]
        });
        myMap.add(mainLayer);
        if (view.zoom >= maxzoom) {
            view.graphics.removeAll();
            drawCover(view.zoom);
        }
    });
}
function rollMap() {
    view.on("mouse-wheel,double-click,pointer-up", function (evt, ent, ect) {
        console.log(evt);
        console.log(view.zoom);
        var viewzoom = view.zoom;
        if (evt.deltaY) {
            if (evt.deltaY > 0) {
                viewzoom--;
            } else {
                viewzoom++;
            }
            setTimeout(function () {
                drawCover(viewzoom);
            }, 1500);
        } else {
            setTimeout(function () {
                drawCover(viewzoom);
            }, 1);
        }
        //drawCover(viewzoom);
    });
}

function drawCover(azoom) {
    //if (selectL == "NB-IoT") return false;
    if (azoom >= maxzoom) {
        var width = $("#viewDiv").width();
        var height = $("#viewDiv").height();
        var p1 = view.toMap(0, 0);
        var p2 = view.toMap(width, height);
        $.ajax({
            url: "/map/GetFinal",
            data: { "minLat": p2.latitude, "minLon": p1.longitude, "maxLat": p1.latitude, "maxLon": p2.longitude, "station": selectL, "type": selectB },
            dataType: "json",
            type: "post",
            success: function (result) {
                if (result.ok) {
                    var table = result.Table;
                    var graphics = new Array();
                    for (var i = 0; i < table.length; i++) {
                        var lineAtt, color, limit, graphic, point;
                        if (selectL == 'CDMA') {
                            point = {
                                type: "point",
                                longitude: table[i].SC_LON,
                                latitude: table[i].SC_LAT
                            };
                            color = [0, 0, 255, 255];
                            var symbol = {
                                type: "simple-marker",
                                path: "M0,10 0,0  -0.65,-1 -1.2,-2 -1.7,-3 -2.1,-4 -2.4,-5 -2.5,-6 -2.3,-7 -1.8,-8 -1,-9 0,-10 1,-9 1.8,-8 2.3,-7 2.5,-6 2.4,-5 2.1,-4 1.7,-3 1.2,-2 0.65,-1 0,0Z",
                                angle: table[i].ANTENNAAZIMUTH,
                                size: 30,
                                outline: { width: 0 },
                                color: color
                            };
                            lineAtt = {
                                "小区名称": table[i].SEC_NAME
                            }
                            graphic = new Graphic({
                                geometry: point,
                                symbol: symbol,
                                attributes: lineAtt,
                                popupTemplate: {
                                    title: "{小区名称}",
                                    content: [{
                                        type: "fields",
                                        fieldInfos: [{ fieldName: "小区名称" }]
                                    }]
                                }
                            });
                        } else {
                            point = {
                                type: "point",
                                longitude: table[i].ENB_LON,
                                latitude: table[i].ENB_LAT
                            };
                            lineAtt = {
                                "小区名称": table[i].SC_NAME,
                                "eNBID&CELLID": table[i].SC_ENBID + "&" + table[i].SC_LCRID,
                                "覆盖热点类型": table[i].HOTSPOTCLASS,
                                "热点名称": table[i].HOTSPOTNAME,
                                "网元自忙时": table[i].BUSYHOUR + "时",
                                "下行流量（GB）": Math.round(table[i].FLOW / 1024 * 100) / 100,
                                "下行PRB利用率%": Math.round(table[i].PRB) + "%",
                                "RRC连接数(个)": table[i].RRC,
                                "覆盖率（RSRP≥-115dB）": (table[i].RSRP ? Math.round(table[i].RSRP * 100) + "%" : ""),
                                "24小时的下行流量数据": table[i].FLOW24,
                                "24小时的下行PRB利用率数据": table[i].PRB24
                            };
                            if (selectB == "流量") {
                                if (selectL == "L800M") {
                                    limit = 1.2;
                                }
                                else if (selectL == "L1.8G") {
                                    limit = 6;
                                }
                                else if (selectL == "L2.1G" || selectL == "L2.6G") {
                                    limit = 8;
                                }
                                else {
                                    limit = 0;//暂定
                                }
                                color = getColor(table[i].FLOW / 1024, limit);
                            } else if (selectB == "PRB利用率") {
                                color = getColor(table[i].PRB, 50);
                            } else if (selectB == "RRC连接数") {
                                color = getColor(table[i].RRC, 200);
                            } else {
                                if (!table[i].RSRP) {
                                    color = [50, 50, 50, 255]
                                }
                                else if (table[i].RSRP >= 0.9) {
                                    color = [0, 255, 0, 255];
                                } else if (table[i].RSRP < 0.9 && table[i].RSRP >= 0.75) {
                                    color = [0, 128, 255, 255];
                                } else if (table[i].RSRP < 0.75 && table[i].RSRP >= 0.6) {
                                    color = [255, 255, 0, 255];
                                } else if (table[i].RSRP < 0.6 && table[i].RSRP >= 0.45) {
                                    color = [255, 128, 0, 255];
                                } else {
                                    color = [255, 0, 0, 255];
                                }
                            }
                            var symbol = {
                                type: "simple-marker",
                                path: "M0,10 0,0  -0.65,-1 -1.2,-2 -1.7,-3 -2.1,-4 -2.4,-5 -2.5,-6 -2.3,-7 -1.8,-8 -1,-9 0,-10 1,-9 1.8,-8 2.3,-7 2.5,-6 2.4,-5 2.1,-4 1.7,-3 1.2,-2 0.65,-1 0,0Z",
                                angle: table[i].ANTENNAAZIMUTH,
                                size: 30,
                                outline: { width: 0 },
                                color: color
                            };
                            graphic = new Graphic({
                                geometry: point,
                                symbol: symbol,
                                attributes: lineAtt,
                                popupTemplate: {
                                    title: "{小区名称}",
                                    content: [{
                                        type: "fields",
                                        fieldInfos: [{
                                            fieldName: "小区名称"
                                        }, {
                                            fieldName: "eNBID&CELLID"
                                        }, {
                                            fieldName: "覆盖热点类型"
                                        }, {
                                            fieldName: "热点名称"
                                        }, {
                                            fieldName: "网元自忙时"
                                        }, {
                                            fieldName: "下行流量（GB）"
                                        }, {
                                            fieldName: "下行PRB利用率%"
                                        }, {
                                            fieldName: "RRC连接数(个)"
                                        }, {
                                            fieldName: "覆盖率（RSRP≥-115dB）"
                                        }, {
                                            fieldName: "24小时的下行流量数据"
                                        }, {
                                            fieldName: "24小时的下行PRB利用率数据"
                                        }]
                                    }]
                                }
                            });
                        }

                        graphics.push(graphic);
                    }
                    view.graphics.removeAll();
                    view.graphics.addMany(graphics);
                }
            },
            error: function () {
                console.log("查询具体坐标失败！");
            }
        });
    } else {
        view.graphics.removeAll();
    }
}

function getColor(num, limit) {
    var color;
    if (num >= limit) {
        color = [255, 0, 0, 255];
    } else if (num >= limit - limit / 4) {
        color = [255, Math.round(255 * ((2 * limit - 2 * (limit / 4)) - num) / (limit - limit / 4)), 0, 255];
    } else if (num >= limit - 2 * (limit / 4)) {
        color = [0, Math.round(255 * (limit - limit / 4 - num) / (limit - 2 * (limit / 4))), 255, 255];
    } else if (num >= limit - 3 * (limit / 4)) {
        color = [0, Math.round(255 * (limit - 2 * (limit / 4) - num + (limit - 2 * (limit / 4)) / 2) / (limit - 2 * (limit / 4))), 255, 255];
    } else {
        color = [0, Math.round(255 * (limit - 3 * (limit / 4) - num + (limit - 3 * (limit / 4)) / 2) / (limit - 3 * (limit / 4))), 0, 255];
    }
    return color;
}


function query() {
    $("#query").click(function () {
        var enbid = $("#enbid").val();
        var cellid = $("#cellid").val();
        if (enbid == null || cellid == null) {
            return false;
        }
        $.ajax({
            url: "/map/query",
            data: { "enbid": enbid, "cellid": cellid, "station": selectL, "type": selectB },
            dataType: "json",
            type: "post",
            success: function (result) {
                if (result.ok) {
                    view.goTo({
                        center: [result.enb_lon, result.enb_lat],
                        zoom: maxzoom
                    });
                    setTimeout(function () {
                        drawCover(maxzoom);
                    },1000);
                    
                } else {
                    $("#query_msg").html("未找到相关信息。(ECI:" + result.eci + ")");
                    setTimeout(function () {
                        $("#query_msg").empty();
                    }, 5000);
                }
            },
            error: function () {
                console.log("查询失败！");
            }
        });
    });
}


