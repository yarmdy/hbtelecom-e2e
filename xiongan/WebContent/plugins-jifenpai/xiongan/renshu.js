

function getnum(){
                
//执行一个AJAX网络请求
	if(jibie=='tian'){
			$.post("xiongan_ajx/renshu.jsp", {
				"index" : index 
			} , function(str){
				//str转换为js可以理解的json对象
               
				//将文本转换为json对象，固定写法，只需关注str这个变量即可
				
				var objArray = eval("("+str+")");
							
				setDigits(parseInt(objArray[0].value));
		        
			});			
			setTimeout(getnum,1000*60*5); 
	}
	else if (jibie=='shi'){
		$.post("xiongan_ajx/renshu_shi.jsp", {
			"index" : index 
		} , function(str){
			//str转换为js可以理解的json对象
           
			//将文本转换为json对象，固定写法，只需关注str这个变量即可
			
			var objArray = eval("("+str+")");
						
			setDigits(parseInt(objArray[0].value));
	        
		});			
		setTimeout(getnum,1000*60*5); 
	}
};

function setDigits(number){
	var num = number+Math.round(Math.random()*200);
	console.log(num);
	var showDigit = "" + num;
	while(showDigit.length<6){
		showDigit = "0" + showDigit;
	}
	$('#jifenpai').remove();
	$("#jifenpai_area").html("<div id=\"jifenpai\"></div>");
	$('#jifenpai').countdown({
        image: 'img/digits.png',
        startTime: showDigit
    });
}