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
                url: "/threemap/get1",
                sublayers: [{
                    name: 3
                }]
            });
            mainLayer1 = new WMSLayer({
                url: "/threemap/get2",
                sublayers: [{
                    name: 3
                }]
            });
            mainLayer2 = new WMSLayer({
                url: "/threemap/get3",
                sublayers: [{
                    name: 3
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
            myMap1 = new Map({
                basemap: customBasemap
            });
            myMap2 = new Map({
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
                url: "/threemap/get1",
                sublayers: [{
                    name: 3
                }]
            });
            mainLayer1 = new WMSLayer({
                url: "/threemap/get2",
                sublayers: [{
                    name: 3
                }]
            });
            mainLayer2 = new WMSLayer({
                url: "/threemap/get3",
                sublayers: [{
                    name: 3
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
            myMap1 = new Map({
                basemap: customBasemap
            });
            myMap2 = new Map({
                basemap: customBasemap
            });
        }
        myMap.add(mainLayer);
        myMap1.add(mainLayer1);
        myMap2.add(mainLayer2);
        view = new MapView({
            center: [116.3836111, 39.9075], // long, lat
            container: "viewDiv",
            map: myMap,
            zoom: minzoom,
            extent: layer.fullExtent
        });
        
        view1 = new MapView({
            center: [116.3836111, 39.9075], // long, lat
            container: "viewDiv1",
            map: myMap1,
            zoom: minzoom,
            extent: layer.fullExtent
        });
        
        view2 = new MapView({
            center: [116.3836111, 39.9075], // long, lat
            container: "viewDiv2",
            map: myMap2,
            zoom: minzoom,
            extent: layer.fullExtent
        });
        
        function all(a, b, c) {
            a.zoom = b.zoom = c.zoom
            a.center = b.center = c.center
        }
        var who = view;
        $('#viewDiv').on('mouseenter', function () { who = view })
        $('#viewDiv1').on('mouseenter', function () { who = view1 })
        $('#viewDiv2').on('mouseenter', function () { who = view2 })
        setInterval(function () {
            if (who === view) {
                all(view1, view2, view)
            }
            if (who === view1) {
                all(view, view2, view1)
            }
            if (who === view2) {
                all(view1, view, view2)
            }
        }, 1000);
    });
    $("#selJD").on("change", function () {
        var vvt = $(this).val();
        myMap.remove(mainLayer);
        myMap1.remove(mainLayer1);
        myMap2.remove(mainLayer2);
        mainLayer = new WMSLayer({
            url: "/threemap/get1",
            sublayers: [{
                name: vvt
            }]
        });
        mainLayer1 = new WMSLayer({
            url: "/threemap/get2",
            sublayers: [{
                name: vvt
            }]
        });
        mainLayer2 = new WMSLayer({
            url: "/threemap/get3",
            sublayers: [{
                name: vvt
            }]
        });
        myMap.add(mainLayer);
        myMap1.add(mainLayer1);
        myMap2.add(mainLayer2);
    });
});
var myMap, myMap1, myMap2, mainLayer, mainLayer1, mainLayer2;