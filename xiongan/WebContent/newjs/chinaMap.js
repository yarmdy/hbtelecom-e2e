var chinaMap;
var inscrease;
var key;
var btperson;
var zxperson;
//左侧地图、折线数据请求
function requestChinaMap(){
	$.ajax({
		url:"serviceJSP/chinaMapData.jsp",
		type:"post",
		data:{"index":index,"month":month},
		dataType:"json",
		success:function(result){
			loadChinaMapData(result);
		},
		error:function(){
			console.log("错误：行政区域数据查询未能正常作用。");
		}
	});
}
//右上角饼图、右下角折线数据请求
function requestPersonData(){
	$.ajax({
		url:"serviceJSP/personData.jsp",
		type:"post",
		data:{"index":index,"month":month},
		dataType:"json",
		success:function(result){
			loadPersonData(result);
		},
		error:function(){
			console.log("错误：任务画像数据查询未能正常作用。");
		}
	});
	
}
//右上角饼图、右下角折线数据处理
function loadPersonData(result){
	var linetime = [];//折线时间轴数据
	var persondata=result.person;
	var tyxlist=result.tyxlist;
	var tttlist=result.tttlist;
	var ttflist=result.ttflist;
	var ftflist=result.ftflist;
	var ftslist=result.ftslist;
	var syslist=result.syslist;
	var nanlist=result.nanlist;
	var nvlist=result.nvlist;
	var timelines = [];
	var alloptions=[];
	var option = {};
	if(month=="all"){
		var date = new Date();
		var year = date.getFullYear();
		var thismonth = date.getMonth()+1;
		for(var i=0;i<thismonth;i++){
			timelines[i] = year+"-"+eval(i+1)+"-01";
			linetime[i] = eval(i+1)+"月";
			var temp = {series:[{label:{normal:{position:"inner"}},type:"pie",top:0,center:["50%","40%"],radius:"30%",data:[{name:"男",value:persondata[i]["nabl"]},{name:"女",value:persondata[i]["nvbl"]}]},{data:[{name:"20岁以下",value:persondata[i]["tyxbl"]},{name:"21~30岁",value:persondata[i]["tttbl"]},{name:"31~40岁",value:persondata[i]["ttfbl"]},{name:"41~50岁",value:persondata[i]["ftfbl"]},{name:"51~60岁",value:persondata[i]["ftsbl"]},{name:"60岁以上",value:persondata[i]["sysbl"]}]}]};//[{geoIndex: 0,data:map[i]}]
			alloptions.push(temp);
		}
		option = {
			timeline:{
				data:timelines,
				label:{
					formatter: function(value){
						var date = new Date(value);
						var text = date.getMonth()+1;
						return text;
					}
				}
			},
			options:alloptions,
		}
		option_chart = {
				tooltip : {
			        trigger: 'axis'
			    },
				legend:{
					data:["男性人数","女性人数","20岁以下","21—30岁","31-40岁","41-50岁","51-60岁","61岁以上"],
					textStyle: {
			            color: '#fff'          // 图例文字颜色
			        }
					
				},
				xAxis:{
					data:linetime
				},
				series:[
					{
						name:"男性人数",
						type:"line",
						data:nanlist
					},
					{
						name:"女性人数",
						type:"line",
						data:nvlist
					},
					{
						name:"20岁以下",
						type:"line",
						data:tyxlist
					},
					{
						name:"21—30岁",
						type:"line",
						data:tttlist
					},
					{
						name:"31-40岁",
						type:"line",
						data:ttflist
					},
					{
						name:"41-50岁",
						type:"line",
						data:ftflist
					},
					{
						name:"51-60岁",
						type:"line",
						data:ftslist
					},
					{
						name:"61岁以上",
						type:"line",
						data:syslist
					},
				]
		}
	}else{
		var date = new Date();
		date.setMonth(eval(month-1));
		date.setDate(1);
		var date2 = new Date(date.getTime()+24*6*60*60*1000);
		for(var i=0;i<5;i++){
			timelines[i] = date2.getFullYear()+"-"+eval(date2.getMonth()+1)+"-"+date2.getDate();
			linetime[i] = "第"+eval(i+1)+"周";
			date2 = new Date(date.getTime()+24*60*60*1000*(7*i+13));
			var temp = {series:[{label:{normal:{position:"inner"}},type:"pie",top:0,center:["50%","40%"],radius:"30%",data:[{name:"男",value:persondata[i]["nabl"]},{name:"女",value:persondata[i]["nvbl"]}]},{data:[{name:"20岁以下",value:persondata[i]["tyxbl"]},{name:"21~30岁",value:persondata[i]["tttbl"]},{name:"31~40岁",value:persondata[i]["ttfbl"]},{name:"41~50岁",value:persondata[i]["ftfbl"]},{name:"51~60岁",value:persondata[i]["ftsbl"]},{name:"60岁以上",value:persondata[i]["sysbl"]}]}]};
			alloptions.push(temp);
		}
		console.log(timelines);
		option = {
				timeline:{
					data:timelines,
					label:{
						formatter: function(value){
							var date = new Date(value);
							var text = eval(date.getMonth()+1)+"-"+date.getDate();
							return text;
						}
					}
				},
				options:alloptions
			}
		option_chart = {
				tooltip : {
			        trigger: 'axis'
			    },
				legend:{
					//data:['男性人数','女性人数','20岁以下','21—30岁','31-40岁'，'41-50岁','51-60岁','61岁以上']	
					data:["男性人数","女性人数","20岁以下","21—30岁","31-40岁","41-50岁","51-60岁","61岁以上"],
					textStyle: {
			            color: '#fff'          // 图例文字颜色
			        }
				},
				xAxis:{
					data:linetime
				},
				series:[
					{
						name:"男性人数",
						type:"line",
						data:nanlist
					},
					{
						name:"女性人数",
						type:"line",
						data:nvlist
					},
					{
						name:"20岁以下",
						type:"line",
						data:tyxlist
					},
					{
						name:"21—30岁",
						type:"line",
						data:tttlist
					},
					{
						name:"31-40岁",
						type:"line",
						data:ttflist
					},
					{
						name:"41-50岁",
						type:"line",
						data:ftflist
					},
					{
						name:"51-60岁",
						type:"line",
						data:ftslist
					},
					{
						name:"61岁以上",
						type:"line",
						data:syslist
					},
				]
		}
	}
	console.log(option);
	btperson.setOption(option);
	zxperson.setOption(option_chart);
}
//左侧地图、折线数据处理
function loadChinaMapData(result){
	var map = result.map;
	var chart = result.chart;
	var timelines = [];//地图时间轴数据
	var option = {};
	var option_chart = {};
	var alloptions=[];//地图数据
	var linetime = [];//折线时间轴数据
	var chartData = [];//折线数据
	if(month=="all"){
		var date = new Date();
		var year = date.getFullYear();
		var thismonth = date.getMonth()+1;
		for(var i=0;i<thismonth;i++){
			timelines[i] = year+"-"+eval(i+1)+"-01";
			linetime[i] = eval(i+1)+"月";
			var temp = {series:[{geoIndex: 0,data:map[i],lineStyle:{normal:{width:3}}}]};
			alloptions.push(temp);
		}
		option = {
			timeline:{
				data:timelines,
				label:{
					formatter: function(value){
						var date = new Date(value);
						var text = date.getMonth()+1;
						return text;
					}
				}
			},
			options:alloptions
		}
		option_chart = {
				tooltip : {
			        trigger: 'axis'
			    },
				xAxis:{
					data:linetime
				},
				series:{
					data:chart
				}
		}
	}else{
		var date = new Date();
		date.setMonth(eval(month-1));
		date.setDate(1);
		var date2 = new Date(date.getTime()+24*6*60*60*1000);
		for(var i=0;i<5;i++){
			timelines[i] = date2.getFullYear()+"-"+eval(date2.getMonth()+1)+"-"+date2.getDate();
			linetime[i] = "第"+eval(i+1)+"周";
			date2 = new Date(date.getTime()+24*60*60*1000*(7*i+13));
			var temp = {series:[{geoIndex: 0,data:map[i]}]};
			alloptions.push(temp);
		}
		option = {
			timeline:{
				data:timelines,
				label:{
					formatter: function(value){
						var date = new Date(value);
						var text = eval(date.getMonth()+1)+"-"+date.getDate();
						return text;
					}
				}
			},
			options:alloptions
		}
		option_chart = {
				xAxis:{
					data:linetime
				},
				series:{
					data:chart
				}
		}
	}
	chinaMap.setOption(option);
	inscrease.setOption(option_chart);
}
//加载左上角中国地图
function loadChinaMap(){
	chinaMap = echarts.init(document.getElementById("chinaMap"));
	var option = {
		baseOption:{
			visualMap: {
                min: 0,
                max: 5000,
                calculable: true,
                inRange: {
                    color: ['#50a3ba', '#eac736', '#d94e5d']
                },
                textStyle: {
                    color: '#000'
                },
                bottom:20
            },
			geo:{
				name:"chinaMap",
				map:"china",
				roam:false,
				top:20,
			},
			timeline:{
    			data:[],//时间轴的日期
    			autoPlay:true,
    			label:{
    				normal:{
    					color:'#FFF',
    					show:true,
    					interval: 'auto',
    					formatter: null
    				}
    			},
    			lineStyle:{
    				color:"#FFF"
    			},
    			controlStyle:{
    				normal:{
    					shhow:false,
    					color:"#FFF",
    					borderColor:"#FFF"
    				}
    			},
    			bottom:30
    		},
    		series:[
    			{
    				type:"map"
    			}
    		]
		},
		options:[
//			{
//				series:[
//    				{
//    					geoIndex: 0,
//    					name:"china",
//    					data:[]//地图的数据
//    				}
//    			]
//			}
		]
	};
	chinaMap.setOption(option);
}
//左下角折线图
function chinaIncreChart(){
	inscrease = echarts.init(document.getElementById("chinaIncre"));
	var option = {
		xAxis:{
			data:[],
			axisLine:{
				lineStyle:{
					color:"#FFF"
				}
			}
		},
		yAxis:{
			axisLine:{
				lineStyle:{
					color:"#FFF"
				}
			}
		},
		series:[
			{
				type:"line",
				data:[]//数据
			}
		]
	}

	inscrease.setOption(option);
}
//加载右上角饼图
function personFigure(){
	btperson = echarts.init(document.getElementById("personFigure"));

	var option = {
		baseOption:{
			timeline:{
    			data:[],//时间轴的日期
    			label:{
    				normal:{
    					color:'#FFF',
    					show:true,
    					interval: 'auto',
    					formatter: '{value}',
    				},
    				emphasis:{
    					color:'#FFF',
    					show:true,
    					interval: 'auto',
    					formatter: '{value}',
    				}
    			},
    			lineStyle:{
    				color:"#FFF"
    			},
    			controlStyle:{
    				normal:{
    					shhow:false,
    					color:"#FFF",
    					borderColor:"#FFF"
    				}
    			},
    			bottom:30
    		},
    		series:[
    			{
    				type:"pie"
    			}
    		]
		},
		options:[
			{
				series:[
			{
				type:"pie",
				top:0,
				center:["50%","40%"],
				radius:"30%",
				data:[{name:"男",value:60},{name:"女",value:40}],
				label:{
                    normal:{
                        position:"inner"
                	}
                }
			},
			{
				type:"pie",
				radius:["45%","65%"],
				center:["50%","40%"],
				data:[
					{name:"20岁以下",value:30},
					{name:"21~30岁",value:10},
					{name:"31~40岁",value:8},
					{name:"41~50岁",value:20},
					{name:"51~60岁",value:12},
					{name:"60岁以上",value:20}
				]
			}
		]
			}
		]
	}

	btperson.setOption(option);
}
//加载中间下部折线图
function keyArea(){
	key = echarts.init(document.getElementById("keyArea"));

	var option = {
		legend:{
			show:true,
			top:10,
			data:[]
		},
		xAxis:{
			data:[],
			axisLine:{
				lineStyle:{
					color:"#FFF"
				}
			}
		},
		yAxis:{
			axisLine:{
				lineStyle:{
					color:"#FFF"
				}
			}
		},
		series:[]
	}

	key.setOption(option);
}
//关键区域数据请求
function requestKeyArea(){
	$.ajax({
		url:"serviceJSP/keyArea.jsp",
		type:"post",
		data:{"index":index,"month":month},
		dataType:"json",
		success:function(data){
			var result = data.data;
			console.log(result);
			var names = data.name;
			var now = new Date();
			var thisMonth = now.getMonth()+1;
			var xData = [];
			var option = {};
			var allSeries = [];
			var nullOption = {};
			var len = result.length;
			console.log("len:"+len);
			var color = ['#f00','#ff0','#f80','#0ff','#0f8'];
			if(month=="all"){
				for(var i=0;i<thisMonth;i++){
					xData[i] = eval(i+1);
				}
				for(var i=0;i<len;i++){
					var temp = {name:names[i],type:"line",lineStyle:{normal:{color:color[i],width:3}},data:result[i]};
					allSeries.push(temp);
				}
				console.log(allSeries);
			}else{
				for(var i=0;i<5;i++){
					xData[i] ="第"+ eval(i+1)+"周";
				}
				for(var i=0;i<len;i++){
					var temp = {name:names[i],type:"line",lineStyle:{normal:{color:color[i],width:3}},data:result[i]};
					allSeries.push(temp);
				}
			}
			for(var i=0;i<names.length;i++){
				names[i] = {name:names[i]};
			}
			nullOPtion = {
				series:[{type:"line",data:[]},{type:"line",data:[]},{type:"line",data:[]},{type:"line",data:[]},{type:"line",data:[]}]
			}
			option = {
				tooltip : {
			        trigger: 'axis'
			    },
				legend:{
					textStyle:{
						color:'#FFF'
					},
					data:names
				},
				xAxis:{
					data:xData
				},
				series:allSeries
			}
			key.setOption(nullOPtion);
			key.setOption(option);
		},
		error:function(){
			console.log("错误：行政区域数据查询未能正常作用。");
		}
	});
}
//加载右下角折线图
function personIcre(){
	zxperson = echarts.init(document.getElementById("personIcre"));

	var option = {
		xAxis:{
			data:[],
			axisLine:{
				lineStyle:{
					color:"#FFF"
				}
			}
		},
		yAxis:{
			axisLine:{
				lineStyle:{
					color:"#FFF"
				}
			}
		},
		series:[
			{
				name:"male",
				type:"line",
				data:[]
			},
			{
				name:"female",
				type:"line",
				data:[]
			},
			{
				name:"under20",
				type:"line",
				data:[]
			},
			{
				name:"under30",
				type:"line",
				data:[]
			},
			{
				name:"under40",
				type:"line",
				data:[]
			},
			{
				name:"under50",
				type:"line",
				data:[]
			},
			{
				name:"under60",
				type:"line",
				data:[]
			},
			{
				name:"above60",
				type:"line",
				data:[]
			},
		]
	}

	zxperson.setOption(option);
}

