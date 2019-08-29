<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="DWServices.page.index" %> 
<!DOCTYPE html>
<html style="overflow: hidden;">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>定位平台</title> 
<link rel="stylesheet" href="css/styles.css" type="text/css" />   
<script type="text/javascript" src="js/jquery-1.12.4.js"></script>   
<link rel="stylesheet" href="frame/libs/bootstrap-3.3.7/css/bootstrap.min.css"> 
<script src="frame/libs/bootstrap-3.3.7/js/bootstrap.min.js"></script>  
<link rel="stylesheet" href="themes/default/style.css">
    <style>
        #goodrate {
            width:160px;
            height:50px;
            position:relative;
        }
		#mapurl {
            width:260px;
            height:50px;
            position:relative;
        }
        .navbar-nav li ul {
            position:absolute
        }
        .navbar-nav li ul li{
            display:none
        }
        .navbar-nav li ul li.tot{
            display:block
        }
        .navbar-nav li ul li a{
            padding: 0px 23px;
    height: 50px;
    line-height: 50px;
    background-color:#00b4fb;
    display:block;
    color:#b2d1ff
        }
            .navbar-nav li ul:hover li {
            display:block
            }
        .navbar-nav {
        margin-left: -100px;
    }
    </style>
  <script>
      function hidebar() {
          $('#sys-navbar').addClass('navbar-hide');
          //显示下拉按钮
          $('#btn-navbar-col').removeClass('hide');
          $('#btn-navbar-col').show();
          $('#btn-navbar-col').click(function () {
              $('#btn-navbar-col').hide();
              $('#sys-navbar').removeClass('navbar-hide');
          });
      } 
      function logout(){
          window.location.href = "../login.ashx?logout=yes";
      }
      function changepwd() {
          $("#oldpwd").val("");
          $("#newpwd").val("");
          $("#newpwd2").val("");
          $("#changepwdbox").modal('show');
      }
      function changpwdconfirm() {
          $.post("../login.ashx?act=cpwd", {
              opwd: $("#oldpwd").val(),
              npwd: $("#newpwd").val(),
              npwd2: $("#newpwd2").val()
          }, function (data) {
              var datajson = eval('('+data+')');
              if (datajson.ok) {
                  alert("修改成功！");
                  $("#changepwdbox").modal('hide');
              } else {
                  alert("修改失败：" + datajson.msg);
                  if (datajson.msg == "会话超时请重新登录") {
                      window.location.href = "../login.ashx?logout=yes";
                  }
              }
          });
      }
      function gotomap(a) {
          var $this = $(a);
          $.ajax({
              async: false,
              cache: false,
              data: { key: "mapurl" },
              success: function (u) {
                  $this.attr("href",u);
              },
              dataType: "text",
              url:"/index.ashx"
          });
      }
      function gotomap2(a) {
          var $this = $(a);
          $.ajax({
              async: false,
              cache: false,
              data: { key: "mapurl2" },
              success: function (u) {
                  $this.attr("href", u);
              },
              dataType: "text",
              url: "/index.ashx"
          });
      }
  </script>
</head> 
 <body  style="overflow: hidden;"> 
    <button id="btn-navbar-col" type="button"
		class="btn btn-link btn-navbar-col hide">
		<span class="glyphicon glyphicon-chevron-down"></span>导航栏
	</button> 
	<nav id="sys-navbar" class="navbar navbar-default" role="navigation">
		<div class="navbar-header">
			<a class="navbar-brand logo" href="#" target="_blank"></a>
		</div>
		<div>
			<ul id="mainnavbar" class="nav navbar-nav"> 
                <%if (((DWServices.Common.User)Session["user"]).Permissions != "3")
                  {
                     %>
                <li id="Li4"><a href="decision.html?r=0"
					    target="mainframe">指挥调度</a></li>
				<li id="appjc"><a href="list.html?r=0"
					target="mainframe">定位</a></li>   
                <%} %>
                <%if (((DWServices.Common.User)Session["user"]).Permissions == "1")
                  { %>
                 <li id="Li2"><a href="casedata.html?r=0"
					target="mainframe">质差案例</a></li>
                    <li id="appfx"><a href="quota.html?r=0"
					target="mainframe">参数设定</a></li>  
                    
                    
                <%} %>
                <%if (((DWServices.Common.User)Session["user"]).Permissions == "2")
                  { %>
                 <li id="Li3"><a href="casedata.html?r=0"
					target="mainframe">质差案例</a></li>
                    
                    
                <%} %>
                <li id="goodrate">
                    <ul>
                        <li class="tot"><a>监控报表</a></li>
                        <li><a href="performance.html?r=0" target="mainframe">性能监控</a></li>
                        <li><a href="citygoodrate.html?r=0" target="mainframe">地市指标监控</a></li>
<li><a href="threenet.html?r=0" target="mainframe">三网对比</a></li>
                        <li id="Li1"><a href="datalog.html?r=0"
					    target="mainframe">入库完整性</a></li>
                        <li><a href="paidan.html?r=0" target="mainframe">派单解决状态</a></li>
<li><a href="http://27.184.196.35:3397/map/evt" target="_blank">事件察看</a></li>

<li><a href="volte-monitoring.html"  target="mainframe">Volte感知监控</a></li>
<li><a href="volte-single-search.html"  target="mainframe">Volte单用户查询</a></li>
<li><a href="volte-terminal-search.html"  target="mainframe">Volte终端查询</a></li>
                    </ul>
                    
                </li>
                <li id="mapurl">
                    <ul>
                        <li class="tot"><a>大屏展示</a></li>
                        <li><a href="#" onclick="gotomap(this)" target="_blank">移动网络性能监控</a></li>
                        <li><a href="#" onclick="gotomap2(this)" target="_blank">三网覆盖对比</a></li>
<li><a href="http://27.184.196.35:3397/goodrate/goodrate" target="_blank">三网感知对比</a></li>
<li><a href="http://27.184.196.35:3397/goodrate/g3h" target="_blank">三网室内感知对比</a></li>
						<li><a href="http://27.184.196.35:3397/alarm"  target="_blank">小区信息呈现</a></li>

                    </ul>
                    
                </li>
			</ul>
		</div>
		<div>
			<ul class="nav navbar-nav navbar-right" style="padding-right: 30px;"> 
               	<li><a ><b><%=((DWServices.Common.User)Session["user"]).Orgname%>:<%=((DWServices.Common.User)Session["user"]).Username%></b></a></li>
				<li class="dropdown"><a href="#" class="dropdown-toggle"
					data-toggle="dropdown"> <span class="glyphicon glyphicon-list"></span>
				</a>
					<ul class="dropdown-menu">  
						<li class="divider"></li>
                        <li><a href="javascript:void(0)" onclick="changepwd()"><span
								class="glyphicon glyphicon-lock"></span>修改密码</a></li> 
						<li><a href="javascript:void(0)" onclick="logout()"><span
								class="glyphicon glyphicon-log-out"></span>退出</a></li> 
					</ul></li>
				<li><a href="javascript:hidebar()"> <span
						class="glyphicon glyphicon-chevron-up"></span></a></li>
			</ul>
		</div>
	</nav>
     <%if (((DWServices.Common.User)Session["user"]).Permissions != "3")
                  {
                     %>
	<iframe id="mainframe" name="mainframe" src="decision.html"
		scrolling="yes" frameborder="0" style="width: 100%; height: 95%;"></iframe>
     <%} %>


     <!-- Modal -->
<div class="modal fade" id="changepwdbox" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <h4 class="modal-title" id="myModalLabel">修改密码</h4>
      </div>
      <div class="modal-body">
        <form class="form-horizontal">
          <div class="form-group">
            <label for="oldpwd" class="col-sm-2 control-label">旧密码</label>
            <div class="col-sm-10">
              <input type="email" class="form-control" id="oldpwd" placeholder="旧密码">
            </div>
          </div>
          <div class="form-group">
            <label for="newpwd" class="col-sm-2 control-label">新密码</label>
            <div class="col-sm-10">
              <input type="password" class="form-control" id="newpwd" placeholder="新密码">
            </div>
          </div>
          <div class="form-group">
            <label for="newpwd2" class="col-sm-2 control-label">确认码</label>
            <div class="col-sm-10">
              <input type="password" class="form-control" id="newpwd2" placeholder="确认密码">
            </div>
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" data-dismiss="modal">取消</button>
        <button type="button" class="btn btn-primary" onclick="changpwdconfirm();">修改</button>
      </div>
    </div>
  </div>
</div>

</body>
</html>
