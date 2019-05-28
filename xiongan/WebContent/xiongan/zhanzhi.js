var mapMarker = [];
var fanLabel = null;
var zhuangtai='kai';
/*var mendian_type={
		"专营店":"★",
		"直营店":"●",
		"合作店":"■",
		"独立店":"▲",
		"连锁店":"◆"
		
	};*/
//function getMendian(lng,lat){
function getzhanzhi(){
	$(".kai").css("background","white");
	$(".kai").css("color","#895DBD");
	$(".guan").css("background","#895DBD");
	$(".guan").css("color","white");
	
	for(var i=0;i<mapMarker.length;i++){
		mapMarker[i].remove();
	}
	$.post("xiongan_ajx/zhanzhi.jsp",{"index" : index },function(re){
		var list = eval("("+re+")");

		for(var i=0;i<list.length;i++){
		
			var point = new BMap.Point(list[i].lng,list[i].lat);
			var fanPts1 = getFanPts(point , 500 , 0); //50是扇区的大小，0是方位角
			var fanPts2 = getFanPts(point , 500, 120); //50是扇区的大小，0是方位角
			var fanPts3 = getFanPts(point , 500 , 240); //50是扇区的大小，0是方位角
			var polygon1 = new BMap.Polygon(fanPts1, {strokeColor:"blue", strokeWeight:2, strokeOpacity:0.5}); 
			var polygon2 = new BMap.Polygon(fanPts2, {strokeColor:"blue", strokeWeight:2, strokeOpacity:0.5}); 
			var polygon3 = new BMap.Polygon(fanPts3, {strokeColor:"blue", strokeWeight:2, strokeOpacity:0.5});
			polygon1.name = list[i].name;
			//绑定鼠标移动上去的事件
			polygon1.addEventListener("mouseover",function(mrk){
				//获取多边形的中心点
				var center = mrk.target.getBounds().getCenter();
				var name = mrk.target.name;
				var opts = { position:center,offset   : new BMap.Size(0, 0)};
				var label = new BMap.Label(name, opts);  // 创建文本标注对象
	 			label.setStyle({
	 				 border:"none",
	 				 color : "#333",
	 				 fontSize : "9px",
	 				 height : "10px",
	 				 lineHeight : "10px"
	 			});
		     	map.addOverlay(fanLabel); 
			});
			
			polygon1.addEventListener("mouseout",function(mrk){
				//移除文字标签
				if(fanLabel!=null)
					map.removeOverlay(fanLabel);
			});
			mapMarker.push(polygon1); 
			mapMarker.push(polygon2); 
			mapMarker.push(polygon3); 
			map.addOverlay(polygon1); 
			map.addOverlay(polygon2); 
			map.addOverlay(polygon3); 
		}
		
		
		
		
//		for(var i=0;i<list.length;i++){
	//		var point = new BMap.Point(list[i].lng,list[i].lat);
		//	var opts = {
	//				  position : point,    // 指定文本标注所在的地理位置
	//				  offset   : new BMap.Size(16, -16)    //设置文本偏移量
	//				}
	//		
	//		var label = new BMap.Label("<div class='marker' style='width:10px;overflow:hidden;font-weight:bold;'><font style='font-size:10px;'>&nbsp;" + mendian_type[list[i].type] + "&nbsp;</font>"+list[i].name+"</div>", opts);  // 创建文本标注对象
	//		var label = new BMap.Label("<div class='marker' style='width:10px;overflow:hidden;font-weight:bold;'><font style='font-size:10px;'>&nbsp;" + "●" + "&nbsp;</font>"+list[i].name+"</div>", opts);  // 创建文本标注对象			
	//		label.setStyle({
	//			border:"none",
	//			background:"none",
	//			 color : "#FF9A00",
	//			 fontSize : "8px",
	//			 height : "16px",
	//			 lineHeight : "16px",
	//			 fontFamily:"微软雅黑",
	//			 zIndex:10000
	//		 });
	//		mapMarker.push(label);
	//		map.addOverlay(label);              // 将标注添加到地图中
	//	}
		
		/*
		setTimeout(function(){
		$(".marker").each(function(index){
			$(".marker").mouseover(function(){
				$(this).css('width','240px');
			});
			$(".marker").mouseout(function(){
				$(this).css('width','10px');
			});
		});
		}
		,500);
		*/
	});
};

function quzhanshi()
{    
	$(".guan").css("background","white");
	$(".guan").css("color","#895DBD");
	$(".kai").css("background","#895DBD");
	$(".kai").css("color","white");
	for(var i=0;i<mapMarker.length;i++){
		mapMarker[i].remove();
	}
	};
	
function kaiguan(){
	if(zhuangtai=='kai'){
		zhuangtai='guan';
		$(".kaiguan").css("background","white");
		$(".kaiguan").css("color","#8EA8E0");
		quzhanshi();
	}
	else if (zhuangtai=='guan'){
		zhuangtai='kai';
		$(".kaiguan").css("background","#8EA8E0");
		$(".kaiguan").css("color","white");
		getzhanzhi();
	}
};
	/*
	获取一个扇区轮廓点
	point:格式为BMap.Point，基站所在的经纬度
	dis:扇区的大小，单位为米
	dir:扇区方位角
	*/
	function getFanPts(point, dis ,dir){
		var points = [];
		points.push(point);
		points.push(GetFanPoint(point,dir+18,dis));
		points.push(GetFanPoint(point,dir+6,dis));
		points.push(GetFanPoint(point,dir-6,dis));
		points.push(GetFanPoint(point,dir-18,dis));
		return points;
	}

	//辅助函数，复制过去就可以，这个函数用于辅助计算，我们不需要直接调用
	function GetFanPoint( p, ang, dis){
		var R = 6378140;
		var arc = ang*Math.PI/180;
		var lng = (Math.sin(arc)*dis/(Math.cos(p.lat*Math.PI/180)*R))*180/Math.PI;
		var lat = ((Math.cos(arc)*dis)/R)*180/Math.PI;
		return new BMap.Point(p.lng+lng,p.lat+lat);
	}
	
	
