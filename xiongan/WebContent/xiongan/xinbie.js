
function getxinbie(){
                
//执行一个AJAX网络请求

			$.post("xiongan_ajx/xinbie.jsp", {
				"index" : index 
			} , function(str){
				//str转换为js可以理解的json对象
               
				//将文本转换为json对象，固定写法，只需关注str这个变量即可
				
				var objArray = eval("("+str+")");	
				setxinbie(objArray);

			
			});
		/*	$.post("tubiao/zhongxia2.jsp", {} , function(str){
				//str转换为js可以理解的json对象
               
				//将文本转换为json对象，固定写法，只需关注str这个变量即可
				
				var objArray = eval("("+str+")");	
				objArray2 = objArray;
			});
			*/
			
			
};

function setxinbie(objArray){
	option2 = {
		    title:{
		        text: '性别',
		        textStyle:{color:'white',fontSize : 15,fontFamily:'微软雅黑'},
		      x:'center',
		      y:50,
		       },
		    tooltip : {
		        trigger: 'item',
		        textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'},
		        formatter: "{a} <br/>{b} : {c} ({d}%)"
		    },
		    series : [
		        {
		            name:'性别比例',
		            type:'pie',
		            radius : '40%',
		            center: ['50%', '60%'],
		            data:objArray,
		            itemStyle : { 
		            	normal : { 
		            	//label : {show : false}, 
		            	label : { 
		            	position : 'outer', 
		            	
		            	}, 
		            	//labelLine : {show : false} 
		            	labelLine : { 
		            	show : true, 
		            	length:1	//线的长度 
		            	} 
		            	}
		            }
		        }
		    ]
		};
		                    
	
		                    
	       
	myChart_xinbie = echarts.init(document.getElementById('xinbie'));
	//2.设置上去
	myChart_xinbie.setOption(option2);
}  