
$(function(){
	//初始化地图
	laiyuanMap = echarts.init(document.getElementById('laiyuanMap'));
	laiyuanChart = echarts.init(document.getElementById('laiyuanChart'),'macarons');
	//加载图标	
	getMapData();
});

function getMapData() {
	   if(jibie=='tian'){
		$.post("xiongan_ajx/laiyuan1.jsp", {
			"index" : index 
		}, function(str) {
			var objArray = eval("(" + str + ")");
			var laiyuanData = [];
			for ( var i = 0; i < objArray.length; i++) {
				laiyuanData.push({
					name : objArray[i].name,		
					value : objArray[i].value
				} );
			}
			loadLaiyuanMap(laiyuanData);
			loadLaiyuanBarChart(laiyuanData);
		});
       }
	   else if (jibie=='shi'){
		   $.post("xiongan_ajx/laiyuan1_shi.jsp", {
				"index" : index 
			}, function(str) {
				var objArray = eval("(" + str + ")");
				var laiyuanData = [];
				for ( var i = 0; i < objArray.length; i++) {
					laiyuanData.push({
						name : objArray[i].name,		
						value : objArray[i].value
					} );
				}
				loadLaiyuanMap(laiyuanData);
				loadLaiyuanBarChart(laiyuanData);
			});
	   }
		
};
function getShengData() {
	if(jibie=='tian'){
	$.post("xiongan_ajx/laiyuan2.jsp", {
		"index" : index
	}, function(str) {
		var objArray = eval("(" + str + ")");
		var laiyuanData = [];

		for ( var i = 0; i < objArray.length; i++) {

			laiyuanData.push({
				name : objArray[i].name,		
				value : objArray[i].value
			} );
		}
		loadShengLaiyuanMap(laiyuanData);
		loadLaiyuanBarChart(laiyuanData);
	});
    }
	else if (jibie=='shi'){
		$.post("xiongan_ajx/laiyuan2_shi.jsp", {
			"index" : index
		}, function(str) {
			var objArray = eval("(" + str + ")");
			var laiyuanData = [];

			for ( var i = 0; i < objArray.length; i++) {

				laiyuanData.push({
					name : objArray[i].name,		
					value : objArray[i].value
				} );
			}
			loadShengLaiyuanMap(laiyuanData);
			loadLaiyuanBarChart(laiyuanData);
		});
	}
	
};


function loadLaiyuanMap(obj){
	 //客流归属地整理

   var guishudiMapData = {
   	lineData:[],
   	markData:[],
   	max: 0
   };
	obj.sort( function(a,b){ return parseInt(b.value)-parseInt(a.value); } );
	for(var i=0;i<obj.length;i++){		
	    guishudiMapData.lineData.push( [{ name: obj[i].name}, { name:index,value:obj[i].value}] );
		guishudiMapData.markData.push(  { name: obj[i].name, value:obj[i].value} );
		if(guishudiMapData.max<parseInt(obj[i].value)){
			guishudiMapData.max=parseInt(obj[i].value);
		}
	}
	var option = {
		    color: ['gold','aqua','lime'],
		    tooltip : {
		        trigger: 'item',
		        formatter: '{b}'
		    },
		    dataRange: {
		        min : 0,
		        max : guishudiMapData.max,
		        calculable : true,
		        show:false,
		        color: ['#ff3333', 'orange', 'yellow','lime','aqua'],
		        textStyle:{
		            color:'#fff'
		        }
		    },
		    series : [
		        {
		            name: '全国',
		            type: 'map',
		            roam: false,
		            hoverable: false,
		            mapType: 'china',
		            itemStyle:{
		                normal:{
		                    borderColor:'rgba(180,149,237,1)',
		                    borderWidth:1,
		                    areaStyle:{
		                        color: '#333367'
		                    }
		                }
		            },
		            data:[],
		            geoCoord: geoLocation
		        },
		        {
		            name: '客流来源',
		            type: 'map',
		            mapType: 'china',
		            data:[],
		            markLine : {
		                smooth:true,
		                effect : {
		                    show: true,
		                    scaleSize: 1,
		                    period: 60,
		                    color: '#fff',
		                    shadowBlur: 10
		                },
		                itemStyle : {
		                    normal: {
			                	label:{show:false},
		                        borderWidth:1,
		                        lineStyle: {
		                            type: 'solid',
		                            shadowBlur: 10
		                        }
		                    }
		                },
		                data : guishudiMapData.lineData
		            },
		            markPoint : {
		                symbol:'emptyCircle',
		                symbolSize : function (v){
		                    return 5 + v/guishudiMapData.max;
		                },
		                effect : {
		                    show: true,
		                    shadowBlur : 0
		                },
		                itemStyle:{
		                    normal:{
		                        label:{show:true}
		                    },
		                    emphasis: {
		                        label:{show:true,position:'top'}
		                    }
		                },
		                data :  guishudiMapData.markData
		            }
		        }
		    ]
		};
	 laiyuanMap.setOption(option,true);                
}

function loadShengLaiyuanMap(obj){
	 //客流归属地整理
 var guishudiMapData = {
 	lineData:[],
 	markData:[],
 	max: 0
 };
	obj.sort( function(a,b){ return parseInt(b.value)-parseInt(a.value); } );
	for(var i=0;i<obj.length;i++){		
	    guishudiMapData.lineData.push( [{ name: obj[i].name}, { name:index,value:obj[i].value}] );
		guishudiMapData.markData.push(  { name: obj[i].name, value:obj[i].value} );
		if(guishudiMapData.max<parseInt(obj[i].value)){
			guishudiMapData.max=parseInt(obj[i].value);
		}
	}
	var option = {
		    color: ['gold','aqua','lime'],
		    tooltip : {
		        trigger: 'item',
		        formatter: '{b}'
		    },
		    dataRange: {
		        min : 0,
		        max : guishudiMapData.max,
		        calculable : true,
		        show:false,
		        color: ['#ff3333', 'orange', 'yellow','lime','aqua'],
		        textStyle:{
		            color:'#fff'
		        }
		    },
		    series : [
		        {
		            name: '全国',
		            type: 'map',
		            roam: false,
		            hoverable: false,
		            mapType: '河北',
		            itemStyle:{
		                normal:{
		                    borderColor:'rgba(180,149,237,1)',
		                    borderWidth:1,
		                    areaStyle:{
		                        color: '#290D4D'
		                    }
		                }
		            },
		            data:[],
		            geoCoord: geoLocation
		        },
		        {
		            name: '客流来源',
		            type: 'map',
		            mapType: '河北',
		            data:[],
		            markLine : {
		                smooth:true,
		                effect : {
		                    show: true,
		                    scaleSize: 1,
		                    period: 60,
		                    color: '#fff',
		                    shadowBlur: 10
		                },
		                itemStyle : {
		                    normal: {
			                	label:{show:false},
		                        borderWidth:1,
		                        lineStyle: {
		                            type: 'solid',
		                            shadowBlur: 10
		                        }
		                    }
		                },
		                data : guishudiMapData.lineData
		            },
		            markPoint : {
		                symbol:'emptyCircle',
		                symbolSize : function (v){
		                    return 5 + v/guishudiMapData.max;
		                },
		                effect : {
		                    show: true,
		                    shadowBlur : 0
		                },
		                itemStyle:{
		                    normal:{
		                        label:{show:true}
		                    },
		                    emphasis: {
		                        label:{show:true,position:'top'}
		                    }
		                },
		                data :  guishudiMapData.markData
		            }
		        }
		    ]
		};
	 laiyuanMap.setOption(option,true);                
}




function loadLaiyuanBarChart(obj){
    var guishudiBarData ={
		name:[],
		data:[]
    };
	obj.sort( function(a,b){ return parseInt(b.value)-parseInt(a.value); } );
	//for(var i=0;i<obj.length;i++){
	//	guishudiBarData.name.push( obj[i].name+'\n'+obj[i].value);
	//	guishudiBarData.data.push( Math.log2(obj[i].value)*Math.log2(obj[i].value));
	//}
	for(var i=0;i<9;i++){
			guishudiBarData.name.push( obj[i].name+'\n'+obj[i].value);
			guishudiBarData.data.push( Math.log2(obj[i].value)*Math.log2(obj[i].value));
		}
	var option = {
		    title: {
		        x: 'center',
		        text: '',
		        subtext: ''
		    },
		    grid:{
		    	borderWidth: 0,
		    	x:0,
		    	y:38,
		    	x2:0,
		    	y2:12
		    },
		    calculable: false,
		    xAxis: [
		        {
		            type: 'category',
		            show: false,
			        axisLabel:{
			        	textStyle: {
			        		color: 'white',
			        		fontFamily:'微软雅黑',
			        		fontSize:10
			        	}
			        },
		            data: guishudiBarData.name
		        }
		    ],
		    yAxis: [
		        {
		            type: 'value',
		            show: false
		        }
		    ],
		    series: [
		        {
		            name: '人数统计',
		            type: 'bar',
		            itemStyle: {
		                normal: {
		                    color: function(params) {
		                        // build a color map as your need.
		                        var colorList = [
		                          '#FF0300','#B5C334','#FCCE10','#E87C25','#27727B',
		                           '#FE8463','#9BCA63','#FAD860','#F3A43B','#60C0DD',
		                           '#D7504B','#C6E579','#F4E001','#F0805A','#26C0C0'
		                        ];
		                        return colorList[params.dataIndex]
		                    },
		                    label: {
		                        show: true,
		                        position: 'top',
		                        formatter: function(p){
		                        	return p.name;
		                        },
		                        textStyle: {
		                        	fontWeight: 'bold',
		                        	color:'white',
		                        	fontSize:10
		                        }
		                    }
		                }
		            },
			        axisLabel:{
			        	textStyle: {
			        		color: 'white',
			        		fontFamily:'微软雅黑',
			        		fontSize:10
			        	}
			        },
		            data: guishudiBarData.data
		        }
		    ]
		};
	 laiyuanChart.setOption(option,true);                 
}