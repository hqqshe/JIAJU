require(["/Content/minjs/config.js"],function(){require(["jquery","pop_window","base","uploadForm","domReady"],function($,_,base,utils,_){base.init(),$("._upload").uploadForm({type:"customer_excel",success:function(_result){eval("var response ="+_result),$.Message({type:"alert",content:response.message}),response.code&&window.location.reload()}}),window._load_items=function(a){$.when(base.loadHtmlData({type:$("._opacity_select").val(),searchKey:$(".search_input").val(),pageIndex:a},"/ERPCustomer/GetCustomerList")).done(function(a){$("._ajax_container").empty().append(a)})},_load_items(1),$("._opacity_select").live("change",function(){_load_items(1)}),$("._ajax_container").on("click",".dele",function(){var a=$(this);$.Message({type:"confirm",content:"你确定删除此客户吗,删除后该客户下面的所有内容将会清除！",okFun:function(){$.when(base.getCodeData({id:a.attr("_id")},"/ERPCustomer/deleteCustomer")).done(function(){$.Message({type:"alert",content:"删除成功！"}),_load_items(1)})}})})})});