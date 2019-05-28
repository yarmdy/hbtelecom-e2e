
function getnlxb(){
                
//执行一个AJAX网络请求

			$.post("xiongan_ajx/nlxb.jsp", {
				"index" : index 
			} , function(str){
				//str转换为js可以理解的json对象
               
				//将文本转换为json对象，固定写法，只需关注str这个变量即可
				
				var objArray = eval("("+str+")");	
				console.log(objArray);
				setnlxb(objArray);
                
			
			});
		/*	$.post("tubiao/zhongxia2.jsp", {} , function(str){
				//str转换为js可以理解的json对象
               
				//将文本转换为json对象，固定写法，只需关注str这个变量即可
				
				var objArray = eval("("+str+")");	
				objArray2 = objArray;
			});
			*/
			
			
};

function setnlxb(objArray){
	option = {
		    tooltip : {
		        trigger: 'item',
		        formatter: "{a} <br/>{b} : {d}%",
		        textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'}
		    },
		    series : [
		        {
		            name:'性别',
		            type:'pie',
		            selectedMode: 'single',
		            radius : [0, 40],
		            
		            // for funnel
		            x: '20%',
		            width: '40%',
		            funnelAlign: 'right',
		          
		            
		            itemStyle : {
		                normal : {
		                    label : {
		                        position : 'inner'
		                    },
		                    labelLine : {
		                        show : false,
		                        
		                    }
		                }
		            },
		            data:[
		                objArray[0],
		                objArray[1],

		            ]
		        },
		        {
		            name:'年龄',
		            type:'pie',
		            radius : [60, 80],
		            
		            // for funnel
		            x: '60%',
		            width: '35%',
		            funnelAlign: 'left',
		            itemStyle : {
		                normal : {
		                   
		                    labelLine : {
		                        
		                        length:5
		                    },
		                    label : {
		                    	textStyle: {
		                        	fontWeight: 'bolder'		                        	
		                        }
		                    }
		                }
		            },
		            
		            data:[
		                objArray[2],
		                objArray[3],
		                objArray[4],
		                objArray[5],
		                objArray[6],
		                objArray[7],
		            ]
		        }
		    ]
		};   		                    
	       
	myChart_nlxb = echarts.init(document.getElementById('nlxb'));
	//2.设置上去
	myChart_nlxb.setOption(option);
}  