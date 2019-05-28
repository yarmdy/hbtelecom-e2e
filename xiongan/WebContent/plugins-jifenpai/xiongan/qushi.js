function getqushi(){
    
	//执行一个AJAX网络请求

				$.post("xiongan_ajx/qushi.jsp", {
					"index" : index 
				} , function(str){
					//str转换为js可以理解的json对象
	               
					//将文本转换为json对象，固定写法，只需关注str这个变量即可
					
					var objArray = eval("("+str+")");	
					console.log(objArray);
					var qushiname = [];
					var qushiData = [];
					var qushidatezong=[];
					var date=[];
					for ( var i = 0; i < objArray.length; i++) {
						qushiData = [];
						qushiname.push(objArray[i].name);
						qushiData.push(objArray[i].value1);
						qushiData.push(objArray[i].value2);
						qushiData.push(objArray[i].value3);
						qushiData.push(objArray[i].value4);
						qushiData.push(objArray[i].value5);
						qushiData.push(objArray[i].value6);
						qushiData.push(objArray[i].value7);
						qushidatezong.push(qushiData);
					};
                   date.push(objArray[0].date1);
                   date.push(objArray[0].date2);
                   date.push(objArray[0].date3);
                   date.push(objArray[0].date4);
                   date.push(objArray[0].date5);
                   date.push(objArray[0].date6);
                   date.push(objArray[0].date7);
                   
                   
					switch(objArray.length){
					    case 10 :
					        loadqushi(qushiname,qushidatezong,date); break;
					    case 1:
						    loadqushi1(qushiname,qushidatezong,date); break;
					    case 2:
					    	loadqushi2(qushiname,qushidatezong,date); break;
					    case 3:
					    	loadqushi3(qushiname,qushidatezong,date); break;
					    case 5:
					    	loadqushi5(qushiname,qushidatezong,date); break;
					};
				
				});
	};

function loadqushi(qushiname,qushidatezong,date){
    
	var option = {
		    tooltip : {
		    	
		        trigger: 'axis',
		      	textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'}
		        	
		    },
		    grid : {
		        x: 50,
		        x2:40
		    },
		    legend: {
		    	x: 'center',
		    	y:  20,
		    	textStyle:{color:'white',fontFamily:'微软雅黑'},
		        data:qushiname
		              },
		    
		    calculable : true,
		    xAxis : [
		        {
		            type : 'category',
		            boundaryGap : false,
		            splitLine: {show: false},
		            //data :[GetDateStr(-7),GetDateStr(-6),GetDateStr(-5),GetDateStr(-4),GetDateStr(-3),GetDateStr(-2),GetDateStr(-1)],//待定
		            data:date,
		            axisLabel: {
						textStyle:{color:'white'},
				               show:true,
				                rotate:0
				                }
		        },
		        
		    ],
		    yAxis : [
		        {
		            type : 'value',
		            scale:true,
		            splitLine: {show: false},
                   // min: Math.min.apply(null, qushidatezong[0]),
                   // max: Math.max.apply(null, qushidatezong[0])+Math.max.apply(null, qushidatezong[1])+Math.max.apply(null, qushidatezong[2])+Math.max.apply(null, qushidatezong[3])+Math.max.apply(null, qushidatezong[4])+Math.max.apply(null, qushidatezong[5])+Math.max.apply(null, qushidatezong[6])+Math.max.apply(null, qushidatezong[7])+Math.max.apply(null, qushidatezong[8])+Math.max.apply(null, qushidatezong[9]),
		            axisLabel : {
						textStyle:{color:'white'},
						show:true,
					}
		        }
		    ],
		    series : [
		        {
		            name:qushiname[0],
		            type:'line',
		            stack: '总量',
		          //  itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[0]
		        },
		        {
		            name:qushiname[1],
		            type:'line',
		            stack: '总量',
		         //   itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[1]
		        },
		        {
		            name:qushiname[2],
		            type:'line',
		            stack: '总量',
		         //   itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[2]
		        },
		        {
		            name:qushiname[3],
		            type:'line',
		            stack: '总量',
		           // itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[3]
		        },
		        {
		            name:qushiname[4],
		            type:'line',
		            stack: '总量',
		          //  itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[4]
		        },
		        {
		            name:qushiname[5],
		            type:'line',
		            stack: '总量',
		         //   itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[5]
		        },
		        {
		            name:qushiname[6],
		            type:'line',
		            stack: '总量',
		         //   itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[6]
		        },
		        {
		            name:qushiname[7],
		            type:'line',
		            stack: '总量',
		         //   itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[7]
		        },
		        {
		            name:qushiname[8],
		            type:'line',
		            stack: '总量',
		         //   itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[8]
		        },
		        {
		            name:qushiname[9],
		            type:'line',
		            stack: '总量',
		        //    itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[9]
		        }
		       
		    ]
		};
	qushi = echarts.init(document.getElementById('qushifenxi'));
	qushi.setOption(option,true);            
};

function loadqushi1(qushiname,qushidatezong,date){
    
	var option = {
		    tooltip : {
		    	
		        trigger: 'axis',
		        textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'}
		    },
		    grid : {
		        x: 50,
		        x2:40
		    },
		    legend: {
		    	x: 'center',
		    	y:  20,
		    	textStyle:{color:'white',fontFamily:'微软雅黑'},
		        data:qushiname
		              },
		    
		    calculable : true,
		    xAxis : [
		        {
		            type : 'category',
		            boundaryGap : false,
		            splitLine: {show: false},
		            //data :[GetDateStr(-7),GetDateStr(-6),GetDateStr(-5),GetDateStr(-4),GetDateStr(-3),GetDateStr(-2),GetDateStr(-1)],//待定
		            data:date,
		            axisLabel: {
						textStyle:{color:'white'},
				               show:true,
				                rotate:0
				                }
		        },
		        
		    ],
		    yAxis : [
		        {
		            type : 'value',
		            scale:true,
		            splitLine: {show: false},
		        //    min: Math.min.apply(null, qushidatezong[0]),
                //    max: Math.max.apply(null, qushidatezong[0]),

		            axisLabel : {
						textStyle:{color:'white'},
						show:true,
					}
		        }
		    ],
		    series : [
		        {
		            name:qushiname[0],
		            type:'line',
		            stack: '总量',
		        //    itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[0]
		        }
		       
		    ]
		};
	qushi = echarts.init(document.getElementById('qushifenxi'));
	qushi.setOption(option,true);            
};

function loadqushi2(qushiname,qushidatezong,date){
    
	var option = {
		    tooltip : {
		    
		        trigger: 'axis',
		        textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'}
		    },
		    grid : {
		        x: 50,
		        x2:40
		    },
		    legend: {
		    	x: 'center',
		    	y:  20,
		    	textStyle:{color:'white',fontFamily:'微软雅黑'},
		        data:qushiname
		              },
		    
		    calculable : true,
		    xAxis : [
		        {
		            type : 'category',
		            boundaryGap : false,
		            splitLine: {show: false},
		            //data :[GetDateStr(-7),GetDateStr(-6),GetDateStr(-5),GetDateStr(-4),GetDateStr(-3),GetDateStr(-2),GetDateStr(-1)],//待定
		            data:date,
		            axisLabel: {
						textStyle:{color:'white'},
				               show:true,
				                rotate:0
				                }
		        },
		        
		    ],
		    yAxis : [
		        {
		            type : 'value',
		            scale:true,
		            splitLine: {show: false},
		        //    min: Math.min.apply(null, qushidatezong[0]),
                 //   max: Math.max.apply(null, qushidatezong[0])+Math.max.apply(null, qushidatezong[1]),
		            axisLabel : {
						textStyle:{color:'white'},
						show:true,
					}
		        }
		    ],
		    series : [
		        {
		            name:qushiname[0],
		            type:'line',
		            stack: '总量',
		         //   itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[0]
		        },
		        {
		            name:qushiname[1],
		            type:'line',
		            stack: '总量',
		         //   itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[1]
		        }
		       
		    ]
		};
	qushi = echarts.init(document.getElementById('qushifenxi'));
	qushi.setOption(option,true);            
};


function loadqushi3(qushiname,qushidatezong,date){
    
	var option = {
		    tooltip : {
		    	
		        trigger: 'axis',
		        textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'}
		    },
		    grid : {
		        x: 50,
		        x2:40
		    },
		    legend: {
		    	x: 'center',
		    	y:  20,
		    	textStyle:{color:'white',fontFamily:'微软雅黑'},
		        data:qushiname
		              },
		    
		    calculable : true,
		    xAxis : [
		        {
		            type : 'category',
		            boundaryGap : false,
		            splitLine: {show: false},
		            //data :[GetDateStr(-7),GetDateStr(-6),GetDateStr(-5),GetDateStr(-4),GetDateStr(-3),GetDateStr(-2),GetDateStr(-1)],//待定
		            data:date,
		            axisLabel: {
						textStyle:{color:'white'},
				               show:true,
				                rotate:0
				                }
		        },
		        
		    ],
		    yAxis : [
		        {
		            type : 'value',
		            scale:true,
		            splitLine: {show: false},
		         //   min: Math.min.apply(null, qushidatezong[0]),
                 //   max: Math.max.apply(null, qushidatezong[0])+Math.max.apply(null, qushidatezong[1])+Math.max.apply(null, qushidatezong[2]),

		            axisLabel : {
						textStyle:{color:'white'},
						show:true,
					}
		        }
		    ],
		    series : [
		        {
		            name:qushiname[0],
		            type:'line',
		            stack: '总量',
		       //     itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[0]
		        },
		        {
		            name:qushiname[1],
		            type:'line',
		            stack: '总量',
		        //    itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[1]
		        },
		        {
		            name:qushiname[2],
		            type:'line',
		            stack: '总量',
		       //     itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[2]
		        }
		       
		    ]
		};
	qushi = echarts.init(document.getElementById('qushifenxi'));
	qushi.setOption(option,true);            
};

function loadqushi5(qushiname,qushidatezong,date){
    
	var option = {
		    tooltip : {
		    	
		        trigger: 'axis',
		      	textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'}
		        	
		    },
		    grid : {
		        x: 50,
		        x2:40
		    },
		    legend: {
		    	x: 'center',
		    	y:  20,
		    	textStyle:{color:'white',fontFamily:'微软雅黑'},
		        data:qushiname
		              },
		    
		    calculable : true,
		    xAxis : [
		        {
		            type : 'category',
		            boundaryGap : false,
		            splitLine: {show: false},
		            //data :[GetDateStr(-7),GetDateStr(-6),GetDateStr(-5),GetDateStr(-4),GetDateStr(-3),GetDateStr(-2),GetDateStr(-1)],//待定
		            data:date,
		            axisLabel: {
						textStyle:{color:'white'},
				               show:true,
				                rotate:0
				                }
		        },
		        
		    ],
		    yAxis : [
		        {
		            type : 'value',
		            scale:true,
		            splitLine: {show: false},
		       //     min: Math.min.apply(null, qushidatezong[0]),
               //     max: Math.max.apply(null, qushidatezong[0])+Math.max.apply(null, qushidatezong[1])+Math.max.apply(null, qushidatezong[2])+Math.max.apply(null, qushidatezong[3])+Math.max.apply(null, qushidatezong[4]),
		            axisLabel : {
						textStyle:{color:'white'},
						show:true,
					}
		        }
		    ],
		    series : [
		        {
		            name:qushiname[0],
		            type:'line',
		            stack: '总量',
		      //      itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[0]
		        },
		        {
		            name:qushiname[1],
		            type:'line',
		            stack: '总量',
		       //     itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[1]
		        },
		        {
		            name:qushiname[2],
		            type:'line',
		            stack: '总量',
		      //      itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[2]
		        },
		        {
		            name:qushiname[3],
		            type:'line',
		            stack: '总量',
		        //    itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[3]
		        },
		        {
		            name:qushiname[4],
		            type:'line',
		            stack: '总量',
		       //     itemStyle: {normal: {areaStyle: {type: 'default'}}},
		            data:qushidatezong[4]
		        }
		       
		    ]
		};
	qushi = echarts.init(document.getElementById('qushifenxi'));
	qushi.setOption(option,true);            
};

/*function GetDateStr(AddDayCount) { 
	var dd = new Date(); 
	dd.setDate(dd.getDate()+AddDayCount);//获取AddDayCount天后的日期 
	var m = dd.getMonth()+1;//获取当前月份的日期 
	var d = dd.getDate(); 
	return m+"-"+d; 
	} 
*/