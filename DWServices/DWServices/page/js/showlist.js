
var dgeoJson = null;
//mr --40 到-140　
//0 到 10000G 
// 0 到 1000个
var paramsss = {
    "MR": {
        defval: "-10000",
        unit: "dbm",
        max: -40,
        min: -140
    },
    "FLOW": {
        defval: "0",
        unit: "gb",
        max: 1000,
        min: 0,
    },
    "LOW": {
        defval: "0",
        unit: "个",
        max: 100,
        min: 0,
    }
}

function showProvince(id, param, key) {
    var selectparam = deepClone(param);
    selectparam.key = key; 
    var myChart = echarts.init(document.getElementById(id));
    $.get("../index.ashx", selectparam, function (result) {
        
     
       
        console.log("result:::", result);
        console.log("paramsss[selectparam.key].defval::", paramsss[selectparam.key].defval);
        var citys = [
                 { name: '石家庄市', value: paramsss[selectparam.key].defval },
                 { name: '承德市', value: paramsss[selectparam.key].defval },
                 { name: '秦皇岛市', value: paramsss[selectparam.key].defval },
                 { name: '唐山市', value: paramsss[selectparam.key].defval },
                 { name: '邯郸市', value: paramsss[selectparam.key].defval },
                 { name: '保定市', value: paramsss[selectparam.key].defval },
                 { name: '张家口市', value: paramsss[selectparam.key].defval },
                 { name: '廊坊市', value: paramsss[selectparam.key].defval },
                 { name: '沧州市', value: paramsss[selectparam.key].defval },
                 { name: '衡水市', value: paramsss[selectparam.key].defval },
                 { name: '邢台市', value: paramsss[selectparam.key].defval }]
        $.get('frame/echarts/map/map-csg.json', function (data) { 
            if (dgeoJson == null) {
                $.ajax({ url: 'frame/echarts/province/hebei.json', async: false }).success(function (geoJson) {
                    dgeoJson = geoJson;
                    console.log("geoJson:::", geoJson);
                });
            }
            console.log("deeL::", dgeoJson);;
            echarts.registerMap("", dgeoJson);
            data.tooltip.formatter = function (param) {
            console.log(param.value)
            return param.name + ":" + (param.value == paramsss[selectparam.key].defval ? '无' : param.value + "(" + paramsss[selectparam.key].unit + ")");
            }
            if (result != "") {
                result = eval("(" + result + ")").data;
                for (var i = 0; i < result.length; i++) {
                    for (var j = 0; j < citys.length; j++) {
                        if (result[i].CITY + "市" == citys[j].name) {
                            citys[j].value = result[i].VALUE;
                        }
                    }
                }
            }
            data.visualMap.min = paramsss[selectparam.key].min;
            data.visualMap.max = paramsss[selectparam.key].max;
            data.series[0].data = citys;
            myChart.setOption(data);
        });
    }) 
}

 