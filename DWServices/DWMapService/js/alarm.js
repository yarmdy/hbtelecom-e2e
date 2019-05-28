$(function () {
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
            "localhost:3399");
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
                url: "/alarm/get",
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
                url: "/alarm/get",
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
    query();
});

var view;


function rollMap() {
    view.on("mouse-wheel,double-click,pointer-up", function (evt, ent, ect) {
        var viewzoom = view.zoom;
        if (evt.deltaY) {
            if (evt.deltaY > 0) {
                viewzoom--;
            } else {
                viewzoom++;
            }
            setTimeout(function () {
                drawCover(viewzoom);
            }, 1000);
        } else {
            setTimeout(function () {
                drawCover(viewzoom);
            }, 1);
        }
    });
}

function drawCover2(azoom) {
    if (azoom >= 1) {
        var graphics = [];
        var lineAtt, color, graphic, point;
        // 点位置
        point = {
            type: "point",
            longitude:120,
            latitude: 30
        }
        // 颜色
        color = "#f00";
        // 图形
        var symbol = {
            type: "simple-marker",
            path: "M0,10 0,0  -0.65,-1 -1.2,-2 -1.7,-3 -2.1,-4 -2.4,-5 -2.5,-6 -2.3,-7 -1.8,-8 -1,-9 0,-10 1,-9 1.8,-8 2.3,-7 2.5,-6 2.4,-5 2.1,-4 1.7,-3 1.2,-2 0.65,-1 0,0Z",
            size: 30,
            outline: { width: 0 },
            color: "#f00"
        };
        // 弹出框内容
        lineAtt = {
            "小区名称": "哈哈哈",
            "城市": "石家庄",
            "名字":"金黄色"
        };
        // 弹出框标题
        graphic = new Graphic({
            geometry: point,
            symbol: symbol,
            attributes: lineAtt,
            popupTemplate: {
                title: "{城市}",
                content: [{
                    type: "fields",
                    fieldInfos: [{ fieldName: "城市" }, { fieldName: "名字"}]
                }]
            }
        });
        graphics.push(graphic);
        view.graphics.removeAll();
        view.graphics.addMany(graphics);
    }
}

function drawCover(azoom) {
    if (azoom >= maxzoom) {
        var width = $("#viewDiv").width();
        var height = $("#viewDiv").height();
        var p1 = view.toMap(0, 0);
        var p2 = view.toMap(width, height);
        $.ajax({
            url: "/alarm/getinfo",
            type: "post",
            data: { "minLat": p2.latitude, "minLon": p1.longitude, "maxLat": p1.latitude, "maxLon": p2.longitude },
            dataType: "json",
            success: function (result) {
                console.log(result);
                if (result.ok) {
                    var table = result.table;
                    
                    var graphics = [];
                    for (var i = 0; i < table.length;i++) {
                        var lineAtt, color, graphic, point;
                        // 点位置
                        point = {
                            type: "point",
                            longitude: table[i].lon,
                            latitude: table[i].lat
                        }
                        // 颜色
                        color = "#f00";
                        // 图形
                        var symbol = {
                            type: "simple-marker",
                            path: "M0,10 0,0  -0.65,-1 -1.2,-2 -1.7,-3 -2.1,-4 -2.4,-5 -2.5,-6 -2.3,-7 -1.8,-8 -1,-9 0,-10 1,-9 1.8,-8 2.3,-7 2.5,-6 2.4,-5 2.1,-4 1.7,-3 1.2,-2 0.65,-1 0,0Z",
                            angle: parseFloat(table[i].angle),
                            size: 30,
                            outline: { width: 0 },
                            color: table[i].bad ? "#f00" : "#0f0"
                        };
                        // 弹出框内容
                        lineAtt = {
                            "城市": table[i].city,
                            "小区名称": table[i].scname,
                            "eNBID&CELLID": table[i].enbid + "&" + table[i].cellid,
                            "ECGI": table[i].eci,
                            "方位角": table[i].angle,
                            "总下倾角": table[i].zangle,
                            "电下倾角": table[i].dangle,
                            "是否故障": table[i].bad ? "是":"否",
                            "故障信息": table[i].badinfo,
                            "RRC连接成功率": table[i].rrc,
                            "上行流量": table[i].upflow,
                            "下行流量": table[i].downflow,
                            "上行PRB利用率": table[i].upprb,
                            "下行PRB利用率": table[i].downprb,
                            "E-RAB掉线率": table[i].erab,
                            "CQI优良比": table[i].cqi,
                        };
                        // 弹出框标题
                        graphic = new Graphic({
                            geometry: point,
                            symbol: symbol,
                            attributes: lineAtt,
                            popupTemplate: {
                                title: "{城市} {小区名称}",
                                content: [{
                                    type: "fields",
                                    fieldInfos: [
                                        { fieldName: "eNBID&CELLID" },
                                        { fieldName: "ECGI" },
                                        { fieldName: "方位角" },
                                        { fieldName: "总下倾角" },
                                        { fieldName: "电下倾角" },
                                        { fieldName: "是否故障" },
                                        { fieldName: "故障信息" },
                                        { fieldName: "RRC连接成功率" },
                                        { fieldName: "上行流量" },
                                        { fieldName: "下行流量" },
                                        { fieldName: "上行PRB利用率" },
                                        { fieldName: "下行PRB利用率" },
                                        { fieldName: "E-RAB掉线率" },
                                        { fieldName: "CQI优良比" },
                                    ]
                                }]
                            }
                        });
                        graphics.push(graphic);
                    }
                    view.graphics.removeAll();
                    view.graphics.addMany(graphics);
                }
            }
        });
    } else {
        view.graphics.removeAll();
    }
}

function query() {
    $("#query").click(function () {
        var enbid = $("#enbid").val();
        var cellid = $("#cellid").val();
        if (enbid == null || cellid == null) {
            return false;
        }
        if (isNaN(parseInt(enbid)) || isNaN(parseInt(cellid))) {
            $("#query_msg").text("输入格式不正确");
            return false;
        }
        $.ajax({
            url: "/alarm/query",
            data: { "enbid": enbid, "cellid": cellid },
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
                    }, 1000);

                } else {
                    $("#query_msg").html("未找到相关信息。");
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

download();

function download() {
    $('#downloadBtn').on('click', function () {
        var no = $('#downloadNum').val();
        $('#readdownload').parent().attr('href', '/alarm/getcitydata?no=' + no);
        $('#readdownload').trigger('click');
    });
}