
		var map;
		var heatmapdata=[];	
		var heatmapdatakong=[];
		function setneimap()
		{
			//执行一个AJAX网络请求
			$.post("xiongan_ajx/reli.jsp", {
				"index" : index 
			} , function(str){
				//str转换为js可以理解的json对象
               
				//将文本转换为json对象，固定写法，只需关注str这个变量即可
				
				var objArray = eval("("+str+")");		
				var reliData = new Array();
				 for ( var i = 0; i < objArray.length; i++) {
				    	var date = new Array();
					    date[0]=parseFloat(objArray[i].value1);
					    date[1]=parseFloat(objArray[i].value2);
					    date[2]=parseFloat(objArray[i].num);
					    reliData.push(date);
				    };
				      
             
                heatmapdata = reliData;
				heatmap(map,heatmapdata);
				getzhanzhi();
			})	;	
			
			initMap();
			
		};
		
		
		
		//页面加载完成之后执行的代码块
		function  initMap(){
			// 创建Map实例
			map = new BMap.Map("map_all");
			// 初始化地图,设置中心点坐标和地图级别 
			switch(index){
			    case "整体"  : 
				   map.centerAndZoom(new BMap.Point(115.944, 38.9682),10);
				   break;
			    case "雄县"  : 
					   map.centerAndZoom(new BMap.Point(116.151, 39.0504),11);
					   break;
			    case "容城"  : 
					   map.centerAndZoom(new BMap.Point(115.838, 39.0847),11);
					   break;
			    case "安新"  : 
					   map.centerAndZoom(new BMap.Point(115.785, 38.8655),11);
					   break;
			    case "白洋淀"  : 
					   map.centerAndZoom(new BMap.Point(116.026, 38.9058),11);
					   break;
			    case "起步区"  : 
					   map.centerAndZoom(new BMap.Point(115.909, 39.0133),12);
					   break;
			};
			
			//添加地图类型控件
			map.addControl(new BMap.MapTypeControl({mapTypes: [BMAP_NORMAL_MAP,BMAP_HYBRID_MAP]})); 
			//开启鼠标滚轮缩放			
			map.enableScrollWheelZoom(true);
			//绑定地图加载完成事件
			map.addEventListener("tilesloaded",function(){
				//alert("地图加载完毕");
				$(".anchorBL").each(function(index){
					var node = this;
					if($(node).html().indexOf("copyright")!=-1||($(node).html().indexOf("Baidu")!=-1&&$(node).html().indexOf("Data")!=-1)){
						$(node).css( "display", "none");
					}
				});
			});
			
			//heatmap(map,heatmapdata);
			map.addEventListener("draggend",function(){
				//console.log(heatmapdata);
				heatmap(map,heatmapdata);
				});
			map.addEventListener("zoomend",function(){
				//console.log(heatmapdata);
				heatmap(map,heatmapdata);
				});
			
			
		};
		
		
		
		 