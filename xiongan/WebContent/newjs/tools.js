var month;
var index;
function selectLocation(){
	index = $("#quyu").val();
	$("#quyu").change(function(){
		month = $("#quyu").val();
		requestChinaMap();
		requestPersonData();
		requestKeyArea();
	});
}

function selectMonth(){
	month = $("#month").val();
	$("#month").change(function(){
		month = $("#month").val();
		requestChinaMap();
		requestPersonData();
		requestKeyArea();
	});
}

function loadMonth(){
	var now = new Date();
	var thisMonth = now.getMonth()+1;
	var monthDoc = "";
	for(var i=1;i<=thisMonth;i++){
		monthDoc += "<option value="+i+">"+i+"月</option>";
	}
	$("#month").append(monthDoc);
}