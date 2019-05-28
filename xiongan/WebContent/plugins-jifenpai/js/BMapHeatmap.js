
//复杂的自定义覆盖物
function ComplexCustomOverlay(point,width,height){
      this._point = point;
      this._width = width;
      this._height = height;
}

function closeHeatMap(_map){
	if(_map.myCompOverlay_luyu!=null){
		_map.myCompOverlay_luyu.remove();
		_map.myCompOverlay_luyu = null;
	}
}

ComplexCustomOverlay.prototype = new BMap.Overlay();
ComplexCustomOverlay.prototype.initialize = function(map){
	  this._map = map;
	  var heatid = parseInt(Math.random()*10000);
	  this._heatid = heatid;
	  var div = this._div = document.createElement("div");
	  div.style.position = "absolute";
	  //div.style.zIndex = BMap.Overlay.getZIndex(this._point.lat);
	  div.style.zIndex = 0;
	  div.style.color = "white";
	  div.style.width = this._width+"px";
	  div.style.height = this._height+"px";
	  div.style.MozUserSelect = "none";
	  div.id = "heat_map"+heatid;
	  var that = this;
	  map.getPanes().labelPane.appendChild(div);
	  return div;
}

ComplexCustomOverlay.prototype.draw = function(){
      var map = this._map;
      var pixel = map.pointToOverlayPixel(this._point);
      this._div.style.left = pixel.x + "px";
      this._div.style.top  = pixel.y + "px";
}
//
function heatmap(_map,heatData){
    //计算热力图半径
	var zoom = _map.getZoom();
	var radius = zoom;
	switch(zoom)
    {
    	
        case 10:radius = 6;break;
        case 11:radius = 10;break;
        case 12:radius = 10;break;
        case 13:radius = 25;break;
        case 14:radius = 36;break;
        case 15:radius = 50;break;
        case 16:radius = 80;break;
        case 17:radius = 100;break;
        case 18:radius = 200;break;
    }
	radius = radius * 1.5;
	var padding= radius *2;
	var center = _map.getCenter();
	var lefttop = _map.pixelToPoint(new BMap.Pixel(0,0));
	var lng_offset = Math.abs(lefttop.lng-center.lng) * 2.5;
	var lat_offset = Math.abs(lefttop.lat-center.lat) * 2.5;
	var bound = new BMap.Bounds(new BMap.Point(center.lng-lng_offset,center.lat-lat_offset),
								new BMap.Point(center.lng+lng_offset,center.lat+lat_offset));
	//console.log(bound);
	//转换成百度地图的点
	var max_x=0,min_x=0,max_y=0,min_y=0;
	var dd = [];
	//计算最大最小点
	for(var i=0; i<heatData.length; i++){
		//将异常数据剔除....//
		var pd = heatData[i];
		var point = new BMap.Point( pd[0], pd[1] );
		if(!bound.containsPoint(point)) continue;
		var pixel = _map.pointToPixel(point);
		if(i==0){
			max_x = pixel.x;
			min_x = pixel.x;
			max_y = pixel.y;
			min_y = pixel.y;
		}
		else{
			if(max_x < pixel.x){
				max_x =pixel.x;
			}
			if(min_x > pixel.x){
				min_x =pixel.x;
			}
			if(max_y < pixel.y){
				max_y =pixel.y;
			}
			if(min_y > pixel.y){
				min_y =pixel.y;
			}
		}
	}
	
	var totalValue = 0;
	for(var i=0; i<heatData.length; i++){
		var pd = heatData[i];
		var pixel = _map.pointToPixel( new BMap.Point( pd[0], pd[1] ));
		var ccccc = 1;
		if(pd.length>2)
			ccccc = pd[2];
		dd.push({x:pixel.x-min_x+padding/2, y:pixel.y-min_y+padding/2, value: pd[2]});
		totalValue+= pd[2];
	}
	
	var width = (max_x - min_x);
	var height = (max_y - min_y);
	var heatMapPoint = _map.pixelToPoint(new BMap.Pixel( min_x, max_y ));
//	alert("minX:"+min_x + " minY:" + min_y + "  maxX:" + max_x + " maxY:" + max_y + " width:"+width + " height:"+height 
//			+"\r\n"+heatMapPoint.lng + " " + heatMapPoint.lat + " ");
	if(_map.myCompOverlay_luyu!=null){
		_map.myCompOverlay_luyu.remove();
		_map.myCompOverlay_luyu = null;
	}
	_map.myCompOverlay_luyu = new ComplexCustomOverlay(  _map.pixelToPoint(new BMap.Pixel( min_x-padding/2, max_y - height-padding/2 )), width+padding, height+padding);
    _map.addOverlay(_map.myCompOverlay_luyu);
    
	var heatmap = h337.create({
	    container: document.getElementById('heat_map'+_map.myCompOverlay_luyu._heatid),
	    radius: radius/2.5,
	    blur: 1,
	    gradient:{
			1:  'rgb(255, 0, 0)',
			.85:'rgb(255, 255, 0)',
			.6: 'rgb(0, 255, 0)',
			.4: 'rgb(0, 0, 255)'
		}
	});
	
	/*
	    gradient:{
			1:'rgb(255, 0, 0)',
			.95:'rgb(255, 255, 0)',
			.7:'rgb(0, 255, 0)',
			.52:'cyan',
			.45:'rgb(0, 0, 255)'
		}
	*/
	var maxValue = (totalValue/heatData.length)*2;
	console.log("流量最大值为:"+maxValue+"MB");
	heatmap.setData({
		max: 10000,
		min:0,
		data: dd
	});
}
