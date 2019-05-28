
function setSulvChart(){
	// original
	var currentSulv = parseInt(Math.random()*15) + 105;
	option = {
		    series : [
		        {
		            name:'速率',
		            type:'gauge',
		            startAngle: 225,
		            endAngle: 0,
		            detail : {formatter:'{value}M'},
		            data:[{value: currentSulv, name: '速率'}],
		            axisLine: {            // 坐标轴线
		                lineStyle: {       // 属性lineStyle控制线条样式
		                	 color: [[0.2, '#794DAD'],[0.8, '#592D8D'],[1, '#390D6D']], 
		                     width: 12
		                }
		            },
		            axisTick: {            // 坐标轴小标记
		            	show:false,
		                splitNumber: 4,   // 每份split细分多少段
		                length :2,        // 属性length控制线长
		                lineStyle: {       // 属性lineStyle控制线条样式
		                    color: 'auto'
		                }
		            },
		            axisLabel: {           // 坐标轴文本标签，详见axis.axisLabel
		            	show:false,
		                textStyle: {       // 其余属性默认使用全局文本样式，详见TEXTSTYLE
		                    color: 'auto'
		                }
		            },
		            splitLine: {           // 分隔线
		                show: true,        // 默认显示，属性show控制显示与否
		                length :0,         // 属性length控制线长
		                lineStyle: {       // 属性lineStyle（详见lineStyle）控制线条样式
		                    color: 'auto'
		                }
		            },
		            pointer : {
		                width : 6
		            },
		            title : {
		                show : false,
		                offsetCenter: [0, '-3%'],       // x, y，单位px
		            },
		            detail : {
		                formatter:'{value}Mbps',
		                textStyle: {       // 其余属性默认使用全局文本样式，详见TEXTSTYLE
		                	fontSize:18,
		                    color: '#fff',
		                    fontWeight:'normal',
		                    fontStyle:'italic'
		                },
	                	offsetCenter: ['12', '-3%'],       // x, y，单位px
		            }
		        }
		    ]
		};
	sulvChart = echarts.init(document.getElementById('sulv'));
	sulvChart.setOption(option);
	setTimeout(setSulvChart,10000);
}