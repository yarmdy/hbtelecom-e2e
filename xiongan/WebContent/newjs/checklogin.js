$.ajax({
	url:"serviceJSP/checkLogin.jsp",
	type:"post",
	dataType:"json",
	success:function(result){
		console.log(result.status);
		var status = result.status;
		if(status == 1){
			window.location = "login.jsp";
		}
	},
	error:function(){
		console.log("登陆检查异常");
	}
});