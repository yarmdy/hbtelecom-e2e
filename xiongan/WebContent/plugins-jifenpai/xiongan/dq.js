function getdq(){
    
	//执行一个AJAX网络请求

				$.post("xiongan_ajx/dq.jsp", {
					"index" : jt 
				} , function(str){
					//str转换为js可以理解的json对象
	               
					//将文本转换为json对象，固定写法，只需关注str这个变量即可
					
					var objArray = eval("("+str+")");						
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

					        loadrenliu(qushiname,qushidatezong,date);
					    
				
				});
	};

function loadrenliu(qushiname,qushidatezong,date){
    
	var option = {
		    tooltip : {
		    	
		        trigger: 'axis',
		        textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'}
		    },
		    grid : {
		        x: 60,
		        x2:45
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
		       /*     itemStyle: {
		                normal: {
		                    label : {
		                        show: true,
		                        position: 'right'
		                    }
		                }
		            },*/
		            data:qushidatezong[0]
		        }
		       
		       
		    ]
		};
	qushi1 = echarts.init(document.getElementById('keliu'));
	qushi1.setOption(option,true);            
};


