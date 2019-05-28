//发送请求
function getqushi(){
	$.post("xiongan_ajx/qushi.jsp", {
		"index" : index 
	} , function(result){
		var data = eval("("+result+")");
		var names = data.names;
		var datas = data.datas;
		var dates = data.dates;
		console.log(data);
		//加载图表
		loadChart(dates);
		//加载数据
		loadData(names,datas);
	});
};
//加载图表
function loadChart(dates){
	var option = {
		    tooltip : {
		        trigger: 'axis',
		        textStyle:{color:'white',fontSize : 10 ,fontFamily:'微软雅黑'}
		    },
		    grid : {
		        x: 50,
		        x2:40
		    },
		    calculable : true,
		    xAxis : [
		        {
		            type : 'category',
		            boundaryGap : false,
		            splitLine: {show: false},
		            data:dates,
		            axisLabel: {
						textStyle:{
							color:'white'
						},
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
						textStyle:{
							color:'white'
						},
						show:true,
					}
		        }
		    ],
		    series : [
		    ]
		};
	qushi = echarts.init(document.getElementById('qushifenxi'));
	qushi.setOption(option,true);
}
//加载数据
function loadData(names,datas){
	var seriesDatas = [];
	for(var i=0;i<names.length;i++){
		var seriesData = {
			name:names[i],
			type:"line",
			stack:"总量",
			data:datas[names[i]]
		};
		seriesDatas.push(seriesData);
	}
	var option = {
		legend: {
	    	x: 'center',
	    	y:  20,
	    	textStyle:{color:'white',fontFamily:'微软雅黑'},
	        data:names
	    },
		series : seriesDatas
	}			
	qushi.setOption(option);
}