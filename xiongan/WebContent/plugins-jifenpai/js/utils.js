$.fn.serializeObject = function()
{
    var o = {};
    var a = this.serializeArray();
    $.each(a, function() {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    
    return o;
};

Date.prototype.pattern=function(fmt) {         
    var o = {         
    "M+" : this.getMonth()+1, //月份         
    "d+" : this.getDate(), //日         
    "h+" : this.getHours()%12 == 0 ? 12 : this.getHours()%12, //小时         
    "H+" : this.getHours(), //小时         
    "m+" : this.getMinutes(), //分         
    "s+" : this.getSeconds(), //秒         
    "q+" : Math.floor((this.getMonth()+3)/3), //季度         
    "S" : this.getMilliseconds() //毫秒         
    };         
    var week = {         
    "0" : "/u65e5",         
    "1" : "/u4e00",         
    "2" : "/u4e8c",         
    "3" : "/u4e09",         
    "4" : "/u56db",         
    "5" : "/u4e94",         
    "6" : "/u516d"        
    };         
    if(/(y+)/.test(fmt)){         
        fmt=fmt.replace(RegExp.$1, (this.getFullYear()+"").substr(4 - RegExp.$1.length));         
    }         
    if(/(E+)/.test(fmt)){         
        fmt=fmt.replace(RegExp.$1, ((RegExp.$1.length>1) ? (RegExp.$1.length>2 ? "/u661f/u671f" : "/u5468") : "")+week[this.getDay()+""]);         
    }         
    for(var k in o){         
        if(new RegExp("("+ k +")").test(fmt)){         
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length==1) ? (o[k]) : (("00"+ o[k]).substr((""+ o[k]).length)));         
        }         
    }         
    return fmt;         
};   

function StrToDate(str){
    str = str.replace(/-/g,"/");
    return new Date(str);
}

function RemoveNullField(json){
    var data=json;
    for(var o in data){
        if((json[o]+'').trim().length==0){
            delete json[o];
        }
    }
    return data;
}

function jq_post(url,json,success){
    $.ajax({ 
        type: "post", 
        url: "FangZu_Get_Ajax.ashx", 
        dataType: "json", 
        data:json,
        success: function (data) { 
                success(data); 
        }, 
        error: function (XMLHttpRequest, textStatus, errorThrown) { 
                alert(errorThrown); 
        } 
    });
}

function nowDate(){
    var myDate = new Date();
    var fy = myDate.getYear();        //获取当前年份(2位)
    var y = myDate.getFullYear();    //获取完整的年份(4位,1970-????)
    var m = myDate.getMonth();       //获取当前月份(0-11,0代表1月)
    var d = myDate.getDate();        //获取当前日(1-31)
    return y+"-"+(m+1)+"-"+d;
}

function nowDateTime(){
    var myDate = new Date();
    var fy = myDate.getYear();        //获取当前年份(2位)
    var y = myDate.getFullYear();    //获取完整的年份(4位,1970-????)
    var m = myDate.getMonth();       //获取当前月份(0-11,0代表1月)
    var d = myDate.getDate();        //获取当前日(1-31)
    return y+"年"+(m+1)+"月"+d +"日"+"时"+"分"+"秒";
}

String.prototype.trim=function(){
    return this.replace(/(^\s*)|(\s*$)/g, "");
};

String.prototype.ltrim=function(){
    return this.replace(/(^\s*)/g,"");
};

String.prototype.rtrim=function(){
    return this.replace(/(\s*$)/g,"");
};

function formatter(date)
{   
    var s = '';
    try{
        s = date.getFullYear()+'-'+(date.getMonth()+1)+'-'+date.getDate()+'';
    }
    catch(e){
        s = nowDate();
    }
    return s;
}

function parser(date){
    if(date.length>0)
        return new Date(Date.parse(date.replace(/-/g,"/")));
    else{
        return new Date();
    }
}

function GetDateDiff(startTime, endTime, diffType) {
    //将xxxx-xx-xx的时间格式，转换为 xxxx/xx/xx的格式 
	if(startTime.indexOf('.')!=-1){
		startTime = startTime.substring(0,startTime.lastIndexOf('.'));
	}
	if(endTime.indexOf('.')!=-1){
		endTime = endTime.substring(0,endTime.lastIndexOf('.'));
	}
    startTime = startTime.replace(/\-/g, "/");
    endTime = endTime.replace(/\-/g, "/");
    //alert(startTime +"   "+ endTime);
    //将计算间隔类性字符转换为小写
    diffType = diffType.toLowerCase();
    var sTime = new Date(startTime);      //开始时间
    var eTime = new Date(endTime);  //结束时间
    //作为除数的数字
    var divNum = 1;
    switch (diffType) {
        case "second":
            divNum = 1000;
            break;
        case "minute":
            divNum = 1000 * 60;
            break;
        case "hour":
            divNum = 1000 * 3600;
            break;
        case "day":
            divNum = 1000 * 3600 * 24;
            break;
        default:
            break;
    }
    return parseInt((eTime.getTime() - sTime.getTime()) / parseInt(divNum));
}

function chinaStrToDateMilliSec( dateStr){//年月日为单位的转换
	dateStr=dateStr.replace("年","/");
	dateStr=dateStr.replace("月","/");
	dateStr=dateStr.replace("日","");
	return Date.parse(dateStr);
}

function SortAsDate(dateArr){
	var list = dateArr;
	for(var i=0;i<list.length;i++){
		for(var j=i+1;j<list.length;j++){
			if(chinaStrToDateMilliSec(list[j])>chinaStrToDateMilliSec(list[i])){
				var tmp = new String(list[i]);
				list[i] = new String(list[j]);
				list[j] = new String(tmp);
			}
		}
	}
}

var base64EncodeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";  
var base64DecodeChars = new Array(  
    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  
    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  
    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,  
    52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,  
    -1,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14,  
    15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,  
    -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,  
    41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1);  
  
function Base64Encode(str) {  
    var out, i, len;  
    var c1, c2, c3;  
  
    len = str.length;  
    i = 0;  
    out = "";  
    while(i < len) {  
    c1 = str.charCodeAt(i++) & 0xff;  
    if(i == len)  
    {  
        out += base64EncodeChars.charAt(c1 >> 2);  
        out += base64EncodeChars.charAt((c1 & 0x3) << 4);  
        out += "==";  
        break;  
    }  
    c2 = str.charCodeAt(i++);  
    if(i == len)  
    {  
        out += base64EncodeChars.charAt(c1 >> 2);  
        out += base64EncodeChars.charAt(((c1 & 0x3)<< 4) | ((c2 & 0xF0) >> 4));  
        out += base64EncodeChars.charAt((c2 & 0xF) << 2);  
        out += "=";  
        break;  
    }  
    c3 = str.charCodeAt(i++);  
    out += base64EncodeChars.charAt(c1 >> 2);  
    out += base64EncodeChars.charAt(((c1 & 0x3)<< 4) | ((c2 & 0xF0) >> 4));  
    out += base64EncodeChars.charAt(((c2 & 0xF) << 2) | ((c3 & 0xC0) >>6));  
    out += base64EncodeChars.charAt(c3 & 0x3F);  
    }  
    return out;  
}  
  
function Base64Decode(str) {  
    var c1, c2, c3, c4;  
    var i, len, out;  
  
    len = str.length;  
    i = 0;  
    out = "";  
    while(i < len) {  
    /* c1 */  
    do {  
        c1 = base64DecodeChars[str.charCodeAt(i++) & 0xff];  
    } while(i < len && c1 == -1);  
    if(c1 == -1)  
        break;  
  
    /* c2 */  
    do {  
        c2 = base64DecodeChars[str.charCodeAt(i++) & 0xff];  
    } while(i < len && c2 == -1);  
    if(c2 == -1)  
        break;  
  
    out += String.fromCharCode((c1 << 2) | ((c2 & 0x30) >> 4));  
  
    /* c3 */  
    do {  
        c3 = str.charCodeAt(i++) & 0xff;  
        if(c3 == 61)  
        return out;  
        c3 = base64DecodeChars[c3];  
    } while(i < len && c3 == -1);  
    if(c3 == -1)  
        break;  
  
    out += String.fromCharCode(((c2 & 0XF) << 4) | ((c3 & 0x3C) >> 2));  
  
    /* c4 */  
    do {  
        c4 = str.charCodeAt(i++) & 0xff;  
        if(c4 == 61)  
        return out;  
        c4 = base64DecodeChars[c4];  
    } while(i < len && c4 == -1);  
    if(c4 == -1)  
        break;  
    out += String.fromCharCode(((c3 & 0x03) << 6) | c4);  
    }  
    return out;  
}  

Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
    if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}