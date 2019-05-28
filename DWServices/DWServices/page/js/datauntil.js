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
//最小值
Array.prototype.min = function () {
    var min = this[0] != null && isNaN(this[0]) == false ? this[0] : -100;
    var len = this.length;
    for (var i = 1; i < len; i++) { 
        if (this[i] != null && isNaN(this[i]) == false && parseInt(this[i]) < parseInt(min)) {
            min = this[i];
        }
        
    }
    return min;
}
//最大值
Array.prototype.max = function () {
    var max = this[0] != null && isNaN(this[0]) == false ? this[0] : -100;
    var len = this.length;
    for (var i = 1; i < len; i++) { 
        if (this[i] != null && isNaN(this[i]) == false && parseInt(this[i]) > parseInt(max)) {
            max = this[i];
        }
    }
    return max;
}

 
function deepClone(obj) {
    var result, oClass = isClass(obj);
    //确定result的类型
    if (oClass === "Object") {
        result = {};
    } else if (oClass === "Array") {
        result = [];
    } else {
        return obj;
    }
    for (key in obj) {
        var copy = obj[key];
        if (isClass(copy) == "Object") {
            result[key] = arguments.callee(copy);//递归调用
        } else if (isClass(copy) == "Array") {
            result[key] = arguments.callee(copy);
        } else {
            result[key] = obj[key];
        }
    }
    return result;
}

//返回传递给他的任意对象的类
function isClass(o) {
    if (o === null) return "Null";
    if (o === undefined) return "Undefined";
    return Object.prototype.toString.call(o).slice(8, -1);
}
var oPerson = {
    oName: "rookiebob",
    oAge: "18",
    oAddress: {
        province: "beijing"
    },
    ofavorite: [
        "swimming",
        { reading: "history book" }
    ],
    skill: function () {
        console.log("bob is coding");
    }
};