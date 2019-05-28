$(function(){
	//设置时间
	startSetTime();
	
    //调用数据接口
    $.get("http://wthrcdn.etouch.cn/weather_mini?citykey=101090217",function(re){
    	var obj = eval("(" + re + ")");    //把字符串转化为对象
    	//console.log(obj);
    	var forecast = obj.data.forecast;
    	var data = obj.data;
    	//alert(JSON.stringify(data));     jason对象转化为字符串输出
    	for(var i=0;i<1;i++){
    		var type = forecast[i].type;
    		var low = forecast[i].low+"";
    		var high = forecast[i].high;
    		var xingqi = forecast[i].date;
    		if(xingqi.indexOf('星期')!=-1){
        		xingqi= xingqi.substring( xingqi.indexOf('星期'), xingqi.length);
    		}
    		var wendu = data.wendu;
    		var city = data.city;
    		var ganmao = data.ganmao;
    		low = low.replace( /低温/g , '');
    		high = high.replace( /高温/g , '');
    		var img_loc = "";
    		var dateName = ["今天","明天","后天"];
    		if(type=='晴')
    			img_loc = 'qing';
    		else if(type=='多云')
        		img_loc = 'duoyun';
    		else if(type.indexOf('雨')!=-1)
        		img_loc = 'yu';
    		else if(type.indexOf('阴')!=-1)
        		img_loc = 'yin';
    		$("#day"+i).html("<table style='height:80%;font-size:6px;color:white;'><tr><td><image src='img/weather/"+img_loc+".png' style='width:54px;'/></td>"
    				+"<td><font style='font-weight:bold;font-size:14px;'>"+"&nbsp"+xingqi+"</br>"+type+ "&nbsp"+wendu+"℃"+"</font>"+"</td></tr></table>");
    	//			+"<td><font style='font-weight:bold;font-size:14px;'>"+ "今天" + "&nbsp"+xingqi+"</br>"+type+ "&nbsp"+wendu+"℃"+"</font>"+"</td></tr></table>");
    	//			+"<td><font style='font-weight:bold;font-size:14px;'>"+type+"</br>"+wendu+"℃"+"</font>"+"</td></tr></table>");
    		$("#tishixinxi").html("<marquee direction='left' >"+""+ ganmao +""+" </marquee>");
    	}
    })
    genxinTime();
    //dianxinrenshu();
    });

function startSetTime(){
	var d = new Date();
	var date = d.pattern("yyyy年MM月dd日");
	var time = d.pattern("hh时mm分ss秒");
	//var time = d.pattern("hh时mm分");
	var xingqi = "星期" + "日一二三四五六".charAt(d.getDay());
	$("#ttt").html("<span style='font-size:15px;'>"+date+"</span>"
		//			+ "&nbsp" + "<span style='font-size:15px;color:yellow;'>"+xingqi+"<br/>"+ "<span style='font-size:12px;color:yellow;'>"+time+"</span>&nbsp;");
			+ "<br/>"+ "<span style='font-size:15px;color:yellow;'>"+time+"</span>&nbsp;");
	
	setTimeout(startSetTime, 1000);
}

function genxinTime(){
	var d = new Date();
	//var date = d.pattern("yyyy年MM月dd日");
	//var time = d.pattern("hh时mm分ss秒");
	var time = d.pattern("hh时mm分");
	//var xingqi = "星期" + "日一二三四五六".charAt(d.getDay());
	$("#tt").html( "<span style='font-size:16px;color:;'>"+"更新时间："+"<br/>"+ "<span style='font-size:14px;color:yellow;'>"+time+"</span>&nbsp;");
		
	
	setTimeout(genxinTime, 1000*60*5);
}

var mapType = 'china';
var jibie='tian';

function togglejibie(jb){
	
	//if(mapType==type)return;
	jibie = jb;
	if(jibie=='tian'){
		$(".tian").css("background","white");
		$(".tian").css("color","#333367");
		$(".shi").css("background","#333367");
		$(".shi").css("color","white");
		//toggleMap(mapType);
		//getnum();
	}
	else if(jibie=='shi'){
		
		$(".tian").css("background","#333367");
		$(".tian").css("color","white");
		$(".shi").css("background","white");
		$(".shi").css("color","#333367");
		//toggleMap(mapType);
		//getnum();
	}
}


function toggleMap(type){
	
	//if(mapType==type)return;
	mapType = type;
	if(mapType=='china'){
		$(".guo").css("background","white");
		$(".guo").css("color","#333367");
		$(".sheng").css("background","#333367");
		$(".sheng").css("color","white");
		$(".shi").css("background","#333367");
		$(".shi").css("color","white");
		getMapData();
	}
	else if(mapType=='hebei'){
		
		$(".guo").css("background","#333367");
		$(".guo").css("color","white");
		$(".sheng").css("background","white");
		$(".sheng").css("color","#333367");
		$(".shi").css("background","#333367");
		$(".shi").css("color","white");
		getShengData();
	}
	else{
		$(".guo").css("background","#333367");
		$(".guo").css("color","white");
		$(".sheng").css("background","#333367");
		$(".sheng").css("color","white");
		$(".shi").css("background","white");
		$(".shi").css("color","#333367");
		getShiData();
	}
}


function dianxinrenshu(){
	
	
   $.post("xiongan_ajx/dxyh.jsp", {
		"index" : index 
	} , function(str){
		//str转换为js可以理解的json对象
       
		//将文本转换为json对象，固定写法，只需关注str这个变量即可
		
		var ob = eval("("+str+")");
		
	var date = ob[0].value;  
	$("#dd").html( "<span style='font-size:15px;color:;'>"+"电信用户数："+"<br/>"+ "<span style='font-size:15px;color:yellow;'>"+date+"</span>&nbsp;");
		
	
	setTimeout(dianxinrenshu, 1000*60*60);
}
	)
}