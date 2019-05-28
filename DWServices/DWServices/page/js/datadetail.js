//展示数据的详情表单

function showDetail() {
    //$(".number").click(function () {
    //    var id = $(this).attr("id");
    //    console.log(id);
    //    var num = $(this).html();
    //    console.log(num);
    //    if (num == 0)
    //    {
    //        return false;
    //    }
    //    $.ajax({
    //        url: "../services/Decision.ashx",
    //        type: "post",
    //        data: { "id": id },
    //        dataType: "json",
    //        success: function (data) {
    //            $("#app").hide();
    //            $("#wifi").hide();
    //            $("#core").hide();
    //            $("#terminal").hide();
    //            if (data.ok) {
    //                $("#dataDetail").modal('show');
    //                $("#web_t").empty();
    //                $("#video_t").empty();
    //                $("#play_t").empty();
    //                $("#single_t").empty();
    //                $("#wifi_t").empty();
    //                $("#core_t").empty();
    //                $("#ter_t").empty();
    //                if (id.indexOf("app") != -1) {
    //                    var c1 = exeWeb(data);
    //                    var c2 = exeVideo(data);
    //                    var c3 = exePlay(data);
    //                    var c4 = exeSingle(data);
    //                    $("#web_t").append(c1);
    //                    $("#video_t").append(c2);
    //                    $("#play_t").append(c3);
    //                    $("#single_t").append(c4);
    //                    $("#app").show();
    //                } else if (id.indexOf("wifi") != -1){
    //                    var c = exeWifi(data);
    //                    $("#wifi_t").append(c);
    //                    $("#wifi").show();
    //                } else if (id.indexOf("core") != -1) {
    //                    var c = exeCore(data);
    //                    $("#core_t").append(c);
    //                    $("#core").show();
    //                } else {
    //                    var c = exeTer(data);
    //                    $("#ter_t").append(c);
    //                    $("#terminal").show();
    //                }
    //            } else {
    //                alert("can't find data");
    //            }
    //        },
    //        error: function (XMLHttpRequest, textStatus, errorThrown) {
    //            alert("No any information");
    //        }
    //    });
    //});
}


//处理web表
function exeWeb(data) {
    var content = "";
    var table;
    var str;
    var webcolor="webcolor";
    if (data.WEB_DAY) {
        table = data.WEB_DAY;
    } else {
        table = data.WEB_MIN;
    }
    content = "<tr><td>开始时间</td><td>web优良率</td><td>应用ID</td><td>服务器</td><td>服务器IP</td><td>WEB业务下行流量（KB）</td>"
        + "<td>WEB业务次数</td><td>页面打开时延质差次数</td><td>首屏打开请求次数</td><td>首屏打开时延质差次数</td></tr>";
    
    for (row = 0; row < table.length; row++) {
        str = "";
        if (table[row].WEBGOODL*1.0<0.8) {
            str="<td class='tdbgcolor'>"+ table[row].WEBGOODL + "</td>";
        }else{
            str="<td>" + table[row].WEBGOODL + "</td>";
        }
        content += ("<tr><td>" + table[row].CREATETIME + "</td>"+str+"<td>" + table[row].APP_NAME + "</td><td>" + table[row].SERVER_NAME
            + "</td><td>" + table[row].IP_ADDRESS + "</td><td>" + table[row].WEB_FLOW + "</td><td>" + table[row].WEB_COUNT + "</td><td>" + table[row].PAGE_RATE
            + "</td><td>" + table[row].SCREEN_RATE + "</td><td>" + table[row].WEB_RATE + "</td></tr>");
        
    }
    return content;
}
//处理video表
function exeVideo(data) {
    var content = "";
    var table;
    var str;
    if (data.VIDEO_DAY) {
        table = data.VIDEO_DAY;
    } else {
        table = data.VIDEO_MIN;
    }
    content = "<tr><td> 开始时间</td><td> 视频优良率</td><td>应用ID</td><td>服务器IP</td><td>视频业务请求次数</td><td>视频下载速率质差次数</td>"
        + "<td>视频播放卡顿质差次数</td></tr>";
    for (row = 0; row < table.length; row++) {
        str = "";
        if (table[row].VIDEODOOGL * 1.0 < 0.8) {
            str = "<td class='tdbgcolor'>" + table[row].VIDEODOOGL + "</td>";
        } else {
            str = "<td>" + table[row].VIDEODOOGL + "</td>";
        }
        content += ("<tr><td>" + table[row].START_TIME + "</td>" + str + "<td>" + table[row].APPLICATION + "</td><td>" + table[row].SERER_IP
            + "</td><td>" + table[row].STREAM_REQUEST + "</td><td>" + table[row].STREAM_DL_GOOD_TIMES + "</td><td>" + table[row].STREAM_HALT_GOOD_TIMES + "</td></tr>");
    }
    return content;
}
//处理play表
function exePlay(data) {
    var content = "";
    var table;
    var str;
    if (data.PLAY_DAY) {
        table = data.PLAY_DAY;
    } else {
        table = data.PLAY_MIN;
    }
    content = "<tr><td>开始时间</td><td>游戏优良率</td><td>应用ID</td><td>服务器</td><td>服务器IP</td>"
        + "<td>游戏业务次数</td><td>游戏时延质差次数</td><</tr>";
    for (row = 0; row < table.length; row++) {
        str = "";
        if (table[row].PLAYGOODL * 1.0 < 0.9) {
            str = "<td class='tdbgcolor'>" + table[row].PLAYGOODL + "</td>";
        } else {
            str = "<td>" + table[row].PLAYGOODL + "</td>";
        }
        content += ("<tr><td>" + table[row].CREATETIME + "</td>" + str + "<td>" + table[row].APP_NAME + "</td><td>" + table[row].SERVER_NAME
            + "</td><td>" + table[row].IP_ADDRESS + "</td><td>" + table[row].PLAY_COUNT + "</td><td>" + table[row].PLAY_RATE + "</td></tr>");
    }
    return content;
}
//处理single表
function exeSingle(data) {
    var content = "";
    var table;
    var str;
    if (data.SIGNAL_DAY) {
        table = data.SIGNAL_DAY;
    } else {
        table = data.SIGNAL_MIN;
    }
    content = "<tr><td>开始时间</td><td>即时通信优良率</td><td>应用ID</td><td>服务器</td><td>服务器IP</td>"
        + "<td>即时通信业务次数</td><td>即时通信发送失败次数</td></tr>";
    for (row = 0; row < table.length; row++) {
        str = "";
        if (table[row].JSTXGOODL * 1.0 < 0.9) {
            str = "<td class='tdbgcolor'>" + table[row].JSTXGOODL + "</td>";
        } else {
            str = "<td>" + table[row].JSTXGOODL + "</td>";
        }
        content += ("<tr><td>" + table[row].CREATETIME + "</td>" + str + "<td>" + table[row].APP_NAME + "</td><td>" + table[row].SERVER_NAME
            + "</td><td>" + table[row].IP_ADDRESS + "</td><td>" + table[row].SIGNAL_COUNT + "</td><td>" + table[row].SIGNAL_FLOW + "</td></tr>");
    }
    return content;
}
//处理core表
function exeCore(data) {
    var content = "";
    var table;
    var strq;
    var strw
    var stre;
    if (data.CORE_DAY) {
        table = data.CORE_DAY;
    } else {
        table = data.CORE_MIN;
    }
    var corecolor = "corecolor";
    content = "<tr><td>开始时间</td><td>附着优良率</td><td>service优良率</td><td>TAU优良率</td><td>MME ID</td><td>附着请求次数</td><td>附着成功次数</td>"
        + "<td>service请求次数</td><td>service成功次数</td><td>TAU请求次数</td>"
        + "<td>TAU成功次数</td></tr>";
    for (row = 0; row < table.length; row++) {
        strq = "";
        if (table[row].FZL * 1.0 < 0.92) {
            strq = "<td class='tdbgcolor'>" + table[row].FZL + "</td>";
        } else {
            strq = "<td>" + table[row].FZL + "</td>";
        }
        strw = "";
        if (table[row].SERVIDEGOOD * 1.0 < 0.92) {
            strw = "<td class='tdbgcolor'>" + table[row].SERVIDEGOOD + "</td>";
        } else {
            strw = "<td>" + table[row].SERVIDEGOOD + "</td>";
        }
        stre = "";
        if (table[row].TAUGOOD * 1.0 < 0.92) {
            stre = "<td class='tdbgcolor'>" + table[row].TAUGOOD + "</td>";
        } else {
            stre = "<td>" + table[row].TAUGOOD + "</td>";
        }
        content += ("<tr><td>" + table[row].START_TIME + "</td>"+strq+strw+stre+"<td>" + table[row].MME_ID + "</td><td>" + table[row].ATTACH_REQUEST
            + "</td><td>" + table[row].ATTACH_SUC + "</td><td>" + table[row].SERVICE_REQUEST + "</td><td>" + table[row].SERVICE_SUC + "</td><td>" + table[row].TAU_REQUEST
            + "</td><td>" + table[row].TAU_SUC + "</td></tr>");
        
    }
    return content;
}
//处理wifi表
function exeWifi(data) {
    var content = "";
    var table;
    var str;
    if (data.KQI_DAY) {
        table = data.KQI_DAY;
    } else {
        table = data.KQI_MIN;
    }
    content = "<tr><td> 开始时间</td><td> 场景</td><td> 小区名称</td><td> 无线优良率</td><td> 城市名称</td><td>城市</td><td>接入网</td>"
        + "<td>ECGI</td><td>首屏显示时延质差次数</td><td>首屏显示次数</td>"
        + "<td>在线游戏时延质差次数</td><td>在线游戏业务次数</td><td>即时通信消息发送请求次数</td>"
        + "<td>即时通信消息发送质差次数</td><td>页面打开时延质差次数</td><td>页面打开总次数</td>"
        + "<td>视频播放速率质差次数</td><td>视频播放卡顿频率质差次数</td><td>视频播放次数</td>"
        + "<td>下行流量</td><td>上行流量</td></tr>";
    for (row = 0; row < table.length; row++) {
        str = "";
        //涉及两个地方
        if (table[row].WIFIGOOD * 1.0 < 0.8) {
            str = "<td class='tdbgcolor'>" + table[row].WIFIGOOD + "</td>";
        } else {
            str = "<td>" + table[row].WIFIGOOD + "</td>";
        }
        content += ("<tr><td>" + table[row].START_TIME + "</td><td>" + table[row].HOTSPOTCLASS + "</td><td>" + table[row].SC_NAME + "</td>" + str + "<td>" + table[row].CITYNAME + "</td><td>" + table[row].CITY + "</td><td>" + table[row].RAT
            + "</td><td>" + table[row].ECGI + "</td><td>" + table[row].FDG_NUM + "</td><td>" + table[row].FD_SUM + "</td><td>" + table[row].GAME_NUM
            + "</td><td>" + table[row].GAME_SUM + "</td><td>" + table[row].NEWS_SUM + "</td><td>" + table[row].NEWS_NUM + "</td><td>" + table[row].PAGE_NUM
            + "</td><td>" + table[row].PAGE_SUM + "</td><td>" + table[row].VIDEO_GNUM + "</td><td>" + table[row].VIDEO_BNUM
            + "</td><td>" + table[row].VIDEO_SNUM + "</td><td>" + Math.round(Number(table[row].BFLOW)/1024/1024*100)/100 + "</td><td>" + Math.round(Number(table[row].TFLOW)/1024/1024*100)/100 + "</td></tr>");
    }
    return content;
}
//处理terminal表
function exeTer(data) {
    var content = "";
    var table = data.TERMINAL_DAY;
    var strq;
    var strw;
    var stre
    var strr;
    var strt;
    var stry;
    content = "<tr><td>开始时间</td><td>WEB优良率</td><td>视频优良率</td><td>即时通信优良率</td><td>游戏优良率</td><td>终端品牌</td><td>终端型号</td><td>首屏质差次数</td><td>首屏次数</td>"
        + "<td>游戏时延质差次数</td><td>游戏业务请求次数</td><td>即时通信发送请求次数</td><td>即时通信发送质差次数</td><td>页面打开时延质差次数</td><td>页面打开请求次数</td>"
        + "<td>视频播放速率质差次数</td><td>视频播放卡顿质差次数</td><td>视频播放请求次数</td></tr> ";
    for (row = 0; row < table.length; row++) {
        strq = "";
        if (table[row].WEBGOODL * 1.0 < 0.7) {
            strq = "<td class='tdbgcolor'>" + table[row].WEBGOODL + "</td>";
        } else {
            strq = "<td>" + table[row].WEBGOODL + "</td>";
        }
        strw = "";
        if (table[row].VIDEODOOGL * 1.0 < 0.7) {
            strw = "<td class='tdbgcolor'>" + table[row].VIDEODOOGL + "</td>";
        } else {
            strw = "<td>" + table[row].VIDEODOOGL + "</td>";
        }
        stre = "";
        if (table[row].JSTXGOODL * 1.0 < 0.7) {
            stre = "<td class='tdbgcolor'>" + table[row].JSTXGOODL + "</td>";
        } else {
            stre = "<td>" + table[row].JSTXGOODL + "</td>";
        }
        strr = "";
        if (table[row].PLAYGOODL * 1.0 < 0.7) {
            strr = "<td class='tdbgcolor'>" + table[row].PLAYGOODL + "</td>";
        } else {
            strr = "<td>" + table[row].PLAYGOODL + "</td>";
        }
       
        content += ("<tr><td>" + table[row].START_TIME + "</td>" + strq +strw+stre+strr+ "<td>" + table[row].BRAND + "</td><td>" + table[row].MODEL
            + "</td><td>" + table[row].FST_SCREEN_GOOD_TIMES + "</td><td>" + table[row].FST_SCREEN_TIMES + "</td><td>" + table[row].GAME_DELAY_GOOD_TIMES + "</td><td>" + table[row].GAME_REQUEST_TIMES
            + "</td><td>" + table[row].IM_SENT_REQUEST_TIMES + "</td><td>" + table[row].IM_SENT_SUC_TIMES + "</td><td>" + table[row].PAGE_OPEN_GOOD_TIMES + "</td><td>" + table[row].PAGE_OPEN_TIMES
            + "</td><td>" + table[row].STREAM_RATE_GOOD_TIMES + "</td><td>" + table[row].STREAM_STALL_GOOD_TIMES + "</td><td>" + table[row].STREAM_REQUEST_TIMES + "</td></tr>");
    }
    return content;
}



//处理IPRAN表
function exeRan(data) {
    var content = "";
    var table = data.IPRAN_DAY;
    var strq;
    var strw;
    var stre
    var strr;
    var strt;
    var stry;
	var json = {};
	
    var question = "";
    content = "<tr><td>开始时间</td><td>城市名称</td><td>关联A设备IP</td><td>A设备名称</td><td>关联B设备IP</td><td>问题描述</td><td>影响小区</td></tr> ";
    for (row = 0; row < table.length; row++) {
		var dr=json[table[row].IPRAN_A];
		if(!dr){
			if (table[row].KIND == "Aping") {
				table[row].question = "A设备故障";
			} else if (table[row].KIND == "Arate"){
				table[row].question = "A设备扩容预警";
			} else {
				table[row].question = "A设备端口流量超过80%";
			}
			json[table[row].IPRAN_A]=table[row];
			json[table[row].IPRAN_A].nameeci="";
		}
		json[table[row].IPRAN_A].nameeci+=table[row].SC_NAME+"("+table[row].ECI+");";
		/*
        if (table[row].KIND == "Aping") {
            question = "A设备故障";
        } else if (table[row].KIND == "Arate"){
            question = "A设备疑似存在问题";
        } else {
            question = "A设备端口流量超过80%";
        }
        content += "<tr><td>" + table[row].CREATEDATE + "</td><td>" + table[row].SC_NAME+"</td><td>" + table[row].CITY + "</td><td>" + table[row].ECI + "</td><td>" + table[row].IPRAN_A + "</td><td>" + table[row].IPRAN_B + "</td><td>" + question + "</td></tr>";
		*/
	}
	
	for (var row in json) {
		content += "<tr><td>" + json[row].CREATEDATE + "</td><td>" + json[row].CITY2 + "</td><td>" + json[row].IPRAN_A + "</td><td>"+json[row].IPRAN_A_NAME+"</td><td>" + json[row].IPRAN_B + "</td><td>" + json[row].question + "</td><td>"+json[row].nameeci+"</td></tr>";
	}
	
	
    return content;
}
function exeFlow(data) {
    var content = "";
    var table;
    var str;
    table = data.FLOW_MIN;
    content = "<tr><td>流量区域</td><td>下行流量(TB)</td><td>上行流量(TB)</td><td>是否达到扩容标准</td></tr>";
    for (row = 0; row < table.length; row++) {
        content += ("<tr><td>河北省</td><td>" + Math.round(table[row].BFLOW / 1024 / 1024 / 1024 / 1024*100)/100 + "</td><td>" + Math.round(table[row].TFLOW / 1024 / 1024 / 1024 / 1024*100)/100 + "</td><td>" + ((parseInt(table[row].BFLOW)+parseInt(table[row].TFLOW))/ 1024 / 1024 / 1024 / 1024<17.58?"否":"是") + "</td></tr>");
    }
    return content;
}