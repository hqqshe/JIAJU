require(["/Content/minjs/config.js"],function(){require(["jquery","pop_window","base","laydate","utils","domReady"],function(a,b,c,b,d,b){function e(){a.when(c.getResponsData({shopId:q},"/ERPOrder/GetCustomerBrandProducttList")).done(function(a){r=a.data,m=a.isExamine,n=a.isLock,f(a.data),h()})}function f(b){if(!b.length)return void a.Message({type:"alert",content:"该客户还未添加品牌！"});for(var c=a("#brand_select ._opacity_select"),d=0;d<b.length;d++){var e=a('<option value="'+b[d].brand.id+'">'+b[d].brand.name+"</option>");c.append(e)}}function g(b){for(var c=a("#pro_select").clone().removeAttr("id").show(),d=a("select",c),e=0;e<b.length;e++){var f=a('<option value="'+b[e].id+'">'+b[e].model_name+"</option>");d.append(f)}return c}function h(){for(var b=a("._original_data ._opacity_select").eq(0).attr("_val"),c=a("._original_data ._opacity_select").eq(1).attr("_val"),d=a("._original_data select:eq(0)"),e=a("._original_data select:eq(1)"),f=0;f<r.length;f++){var h=a('<option value="'+r[f].brand.id+'">'+r[f].brand.name+"</option>");if(b==r[f].brand.id){h.attr("selected","selected");for(var j=0;j<r[f].product.length;j++){var k=a('<option value="'+r[f].product[j].id+'">'+r[f].product[j].model_name+"</option>");r[f].product[j].id==c&&k.attr("selected","selected"),e.append(k)}}d.append(h)}a("._original_data input:eq(1)").bind("click",function(){a("#laydate").attr("id",""),a(this).attr("id","laydate"),laydate({elem:"#laydate",format:"YYYY-MM-DD",isclear:!1,istime:!1,istoday:!1,choose:function(a){}})});var l=a("._original_data");d.bind("change",function(){var b=a(this).val();if(l.find("input").val(""),l.find("td").each(function(b,c){1!=b&&2!=b&&3!=b&&4!=b&&5!=b&&7!=b&&8!=b||a(c).empty()}),b!=-1)for(var c=0;c<r.length;c++)if(r[c].brand.id==b){var d=g(r[c].product);l.find("td:eq(1)").append(d),d.find("select").bind("change",function(){var b=a(this).val();if(l.find("input").val(""),l.find("td").each(function(b,c){2!=b&&3!=b&&4!=b&&5!=b&&7!=b&&8!=b||a(c).empty()}),b!=-1)for(var d=0;d<r[c].product.length;d++)if(r[c].product[d].id==b){i(l,r[c].product[d],r[c].customerPrice[d]);break}});break}}),e.bind("change",function(){var c=a(this).val();if(l.find("input").val(""),l.find("td").each(function(b,c){2!=b&&3!=b&&4!=b&&5!=b&&7!=b&&8!=b||a(c).empty()}),c!=-1)for(var d=0;d<r.length;d++)if(r[d].brand.id==b){for(var e=0;e<r[d].product.length;e++)if(r[d].product[e].id==c){i(l,r[d].product[e],r[d].customerPrice[e]);break}break}}),a(".dele",l).bind("click",function(){var b=a(this).parent().parent();a.Message({type:"confirm",content:"您确定要删除该条信息?",okFun:function(){s=b.attr("_id"),b.remove()}})})}function i(b,c,d){a(b).find("td").each(function(b,e){2==b?(a(e).text(c.format),a(e).attr("model",c.produce_name),a(e).attr("unit",c.unit)):3==b?a(e).text(c.color):4==b?a(e).text(c.standard_price):5==b?a(e).text(d):7==b?a(e).text(c.toal_lock_num):8==b?a(e).text(c.total_avaible_num):10==b?a(e).find("input[name=lock_store]").eq(n).attr("checked","true"):11==b&&a(e).find("input[name=check]").eq(m).attr("checked","true")})}function j(){var b=[];return a(".brand_tab tbody tr").each(function(c,d){var e={};"undefined"!=typeof a(d).attr("_id")?e.id=a(d).attr("_id"):e.id=0,a("td",a(d)).each(function(b,c){0==b?(e.brand_id=a(c).find("select").val(),e.brand_name=a(c).find("._select_text").text()):1==b?(e.product_id=a(c).find("select").val(),e.product_model=a(c).find("._select_text").text()):2==b?(e.product_format=a(c).text(),e.product_name=a(c).attr("model"),e.unit=a(c).attr("unit")):3==b?e.product_color=a(c).text():5==b?e.product_price=a(c).text():10==b?e.lockStock=a(c).find("input[type='radio']:checked").parent().index():11==b&&(e.checkFiance=a(c).find("input[type='radio']:checked").parent().index())}),a("input",a(d)).each(function(b,c){0==b?e.product_number=a(c).val():1==b&&(e.factory_delivery_day=a(c).val())}),e.remarks=a("textarea",a(d)).val(),b.push(e)}),b}function k(){var b={};return b.deleteId=s,b.shopId=q,b.shopName=a("._input_panel .name").text(),b.orderId=a(".order_num").val(),b.factory_consumer_address=a("#address").val(),b.factory_consumer_postcode=a("#post_code").val(),b.factory_consumer_linkman=a("#link_man").val(),b.factory_consumer_phone=a("#phone").val(),b.orderInfo=j(),b}function l(b){if(""==b.orderId)return a.Message({type:"alert",content:"请输入订单信息！"}),!1;if(0==b.orderInfo.length)return a.Message({type:"alert",content:"请添加至少一条产品信息！"}),!1;for(var c=!0,d=0;d<b.orderInfo.length;d++){if(b.orderInfo[d].brand_id==-1){a.Message({type:"alert",content:"第"+(d+1)+"件产品没有选择品牌！"}),c=!1;break}if(b.orderInfo[d].product_id==-1){a.Message({type:"alert",content:"第"+(d+1)+"件产品没有选择产品！"}),c=!1;break}if(""==b.orderInfo[d].product_number){a.Message({type:"alert",content:"第"+(d+1)+"件产品没有输入数量！"}),c=!1;break}if(""==b.orderInfo[d].factory_delivery_day){a.Message({type:"alert",content:"第"+(d+1)+"件产品没有输入交货日期！"}),c=!1;break}}return c}c.init();var m,n,o=' <tr class="select">                                                                                                                                                                           <td></td>                                                                                                                                                                                             <td></td>                                                                                                                                                                                             <td></td>                                                                                                                                                                                             <td></td>                                                                                                                                                                                             <td></td>                                                                                                                                                                                             <td></td>                                                                                                                                                                                             <td><input type="text" placeholder="请填写数量" onkeyup="this.value = this.value.replace(/D/g, \'\')" onafterpaste="    this.value = this.value.replace(/D/g, \'\')"></td>                                            <td></td>                                                                                                                                                                                            <td></td>                                                                                                                                                                                            <td><input type="text" placeholder="请选择交货日期"></td>                                                                                <td> <form>                                                                                                                                     <label><input type="radio" name="lock_store" value="0" >是</label>                                                                  <label><input type="radio" name="lock_store" value="1" >否</label></form>                                             </td>                                                                                                                                                                                                     <td><form>                                                                                                                                      <label><input type="radio" name="check" value="0" >是</label>                                                                           <label><input type="radio" name="check" value="1">否</label></form>                                                        </td>                                                                                                                                                                                                     <td class="big_input"><textarea></textarea></td>                                                                                                         <td><a href="javascript:;" class="dele">×</a></td>                                                                                                     </tr>',p=window.location.search.split("&"),q=p[1].split("=")[1],r=(p[0].split("=")[1],{}),s=0;e(),a(".add_brand").on("click",function(){var b=a(o),c=a("#brand_select").clone().removeAttr("id").show();b.children().first().append(c),a("input:eq(1)",b).bind("click",function(){a("#laydate").attr("id",""),a(this).attr("id","laydate"),laydate({elem:"#laydate",format:"YYYY-MM-DD",isclear:!1,istime:!1,istoday:!1})}),a(".dele",b).bind("click",function(){var b=a(this).parent().parent();a.Message({type:"confirm",content:"您确定要删除该条信息?",okFun:function(){b.remove()}})}),c.find("select").bind("change",function(){var c=a(this).val();if(b.find("input").val(""),b.find("td").each(function(b,c){1!=b&&2!=b&&3!=b&&4!=b&&5!=b&&7!=b&&8!=b||a(c).empty()}),c!=-1)for(var d=0;d<r.length;d++)if(r[d].brand.id==c){var e=g(r[d].product);b.find("td:eq(1)").append(e),e.find("select").bind("change",function(){var c=a(this).val();if(b.find("input").val(""),b.find("td").each(function(b,c){2!=b&&3!=b&&4!=b&&5!=b&&7!=b&&8!=b||a(c).empty()}),c!=-1)for(var e=0;e<r[d].product.length;e++)if(r[d].product[e].id==c){i(b,r[d].product[e],r[d].customerPrice[e]);break}});break}}),a(".brand_tab tbody").append(b)}),a(".sub_mit").on("click",function(){var b=k();l(b)&&(b.orderInfo=d.json2str(b.orderInfo),a.when(c.getCodeData(b,"/ERPOrder/editOrder","post")).done(function(){window.location.href="/ERPOrder/OrderList"}))})})});