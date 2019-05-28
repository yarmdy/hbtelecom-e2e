

function getnjiaotong(){
                
//执行一个AJAX网络请求

			$.post("xiongan_ajx/jiaotong.jsp", {
			} , function(str){
				//str转换为js可以理解的json对象
               
				//将文本转换为json对象，固定写法，只需关注str这个变量即可
				
				var objArray = eval("("+str+")");	
				var data = [];
				for(var i=0; i < objArray.length; i++){
									
									var innerObj = objArray[i];
									
									var sheng = innerObj.name;
									var di = innerObj.value1;
									var ren = innerObj.value2;
									var pai = innerObj.value3;
									data.push([sheng,di,ren,pai]);
								}
				loaddata(data);

			
			});
		/*	$.post("tubiao/zhongxia2.jsp", {} , function(str){
				//str转换为js可以理解的json对象
               
				//将文本转换为json对象，固定写法，只需关注str这个变量即可
				
				var objArray = eval("("+str+")");	
				objArray2 = objArray;
			});
			*/
			
			
};

function loaddata(jt_value){
		
	var data2=jt_value;
	
		
	

		for(var i=0; i<data2.length; i++){
		    var line = data2[i];
		
		//执行代码
		//往id为table_1的元素里动态的添加内容
		$("#table_2").append(
				'<tr style="font-size:14px;" onclick="jtqs(\''+line[0]+'\')">'+
				'<td >'+line[0]+'</td>'+
				'<td >'+line[1]+'</td>'+
				'<td >'+line[2]+'</td>'+
				'<td >'+line[3]+'</td>'+						
			'</tr>');
		}

}