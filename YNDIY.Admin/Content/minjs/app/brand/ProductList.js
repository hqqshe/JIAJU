require(["/Content/minjs/config.js"],function(){require(["jquery","pop_window","base","utils","domReady"],function(a,b,c,d,b){function e(b){var c=a(".parts_con tbody");if(c.empty(),0!==b.length)for(var d=0;d<b.length;d++){var e=a(g);e.children().eq(0).children().val(b[d].name),e.children().eq(1).children().val(b[d].format),e.children().eq(2).children().val(b[d].number),e.children().eq(3).children().val(b[d].material),c.append(e)}}c.init();var f,g='<tr>                                                                                                                                                                                                                                                                                                                                                                                                             <td><input type="text" placeholder="部件名称" class="_input_area"></td>                                                                                                                                                                                                                                                         <td><input type="text" placeholder="规格" class="_input_area"></td>                                                                                                                                                                                                                                                                   <td><input type="text" placeholder="数量" class="_input_area" onkeyup="this.value = this.value.replace(/\\D/g, \'\')" onafterpaste="this.value = this.value.replace(/\\D/g,\'\')"></td>                                         <td><input type="text" placeholder="材料" class="_input_area"></td>                                                                                                                                                                                                                                                                   <td><a href="javascript:;" class="dele">×</a></td>                                                                                                                                                                                                                                                                                                         </tr>';window._load_items=function(b){a.when(c.loadHtmlData({brandId:a("#brandId").val(),productName:a(".search_input").val(),pageIndex:b},"/ERPBrand/GetProductList")).done(function(b){a("._ajax_container").empty().append(b)})},_load_items(1),a("._btns").on("click","a",function(){a(this).index()}),a(".add_part").on("click",function(){a(".parts_con ._list_table tbody").append(a(g).clone())}),a(".parts_con ._list_table").on("click",".dele",function(){a(this).parent().parent().remove()}),a(".parts_con").on("click",".sub_mit",function(){var b=[],e=!1;a(".parts_con tbody tr").each(function(c,d){var f={};if(f.name=a(d).find("input").eq(0).val(),!f.name)return a.Message({type:"alert",content:"请输入部件名称！"}),e=!0,!1;for(var g=0;g<b.length;g++)if(b[g].name===f.name)return a.Message({type:"alert",content:"部件名称"+f.name+"已存在,请修改！"}),e=!0,!1;return f.format=a(d).find("input").eq(1).val(),f.format?(f.number=a(d).find("input").eq(2).val(),f.number?(f.material=a(d).find("input").eq(3).val(),f.material?void b.push(f):(a.Message({type:"alert",content:"请输入部件材料！"}),e=!0,!1)):(a.Message({type:"alert",content:"请输入部件数量！"}),e=!0,!1)):(a.Message({type:"alert",content:"请输入部件规格！"}),e=!0,!1)}),e||a.when(c.getCodeData({productId:f,partsList:d.json2str(b)},"/ERPBrand/savePartsInfo","post")).done(function(){a.Message({type:"alert",content:"提交成功！"}),a(".parts_con").fadeOut()})}),a("._ajax_container").on("click",".view",function(){var b=a(this).attr("_id"),c=a(".search_input").val(),d=a("._opacity_select").val();window.location.href="/ERPBrand/ProductDetail?id="+b+"#"+d+"_"+c}),a("._ajax_container").on("click",".edit_part",function(){f=a(this).attr("_id"),a.when(c.getResponsData({id:f},"/ERPBrand/getPartsInfo")).done(function(b){e(b.partsList),a(".parts_con").fadeIn()})}),a("._ajax_container").on("click",".dele",function(){var b=a(this);a.Message({type:"confirm",content:"你确定删除此产品吗?删除后该产品下面的所有内容将会清除！",okFun:function(){a.when(c.getCodeData({id:b.attr("_id")},"/ERPBrand/DeleteProduct")).done(function(){a.Message({type:"alert",content:"删除成功！"}),_load_items(1)})}})})})});