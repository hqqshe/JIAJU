require(["/Content/minjs/config.js"],function(){require(["jquery","pop_window","base","domReady"],function(a,b,c,b,b){function d(a){for(var b=0,c=0;c<a.length;c++)b+=a[c];return b}c.init();window.location.search.split("=")[1];a("._list_table input").on("blur",function(){var b=+a(this).val(),c=+a(this).parent().prev().text();if(b&&c<b)return a.Message({type:"alert",content:"超过限定数量"}),void a(this).val("")}),a(".sub_mit").on("click",function(){var b=(a(this),[]),e=[];a(".tb1 tbody tr").each(function(c,d){var f=+a(d).children().eq(8).children().val();f&&(b.push(a(d).attr("_id")),e.push(f))});var f=[],g=[];a(".tb2 tbody tr").each(function(b,c){var d=+a(c).children().eq(5).children().val();d&&(f.push(a(c).attr("_id")),g.push(d))});var h=Math.abs(+a("._list_table:eq(0) tbody tr").children().eq(9).text()),i=d(e)+d(g);if(h!==i)return void a.Message({type:"alert",content:"待处理数量为:"+h+" 当前已处理:"+i});var j=a(this).attr("_id");a.when(c.getCodeData({product_id:j,sale_id_str:b.join(","),sale_count_str:e.join(","),outIds:f.join(","),out_count_str:g.join(",")},"/ERPStore/HandleInventory")).done(function(){a.Message({type:"alert",content:"保存成功！"}),window.location.reload()})})})});