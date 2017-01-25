require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'utils', 'pinyin', 'pop_window', 'laydate', 'domReady'], function ($, base, utils, pinyin) {
        base.init();
        var order_item = ' <tr class="select">                                                                                                                               \
                                            <td></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                           <td  class="_td_20 _over_text" ></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                            <td  class="_td_10 _over_text" ><input type="text"placeholder="数量" onkeyup="this.value = this.value.replace(/\D/g, \'\')" onafterpaste="    this.value = this.value.replace(/\D/g, \'\')"></td>\
                                            <td></td>                                                                                                                                                \
                                            <td></td>                                                                                                                                                \
                                            <td  class="_td_10 _over_text" ><input type="text" placeholder="交货日期" style="text-indent:5px;background-image:none"></td>                                    \
                                            <td> <form>                                                                                     \
                                                <label><input type="radio" name="lock_store" value="0" >是</label>                  \
                                                <label><input type="radio" name="lock_store" value="1" >否</label></form> \
                                            </td>                                                                                                                                                         \
                                            <td><form>                                                                                      \
                                                <label><input type="radio" name="check" value="0" >是</label>                           \
                                                <label><input type="radio" name="check" value="1">否</label></form>            \
                                            </td>                                                                                                                                                         \
                                            <td class="big_input"><textarea></textarea></td>                                                             \
                                            <td><a href="javascript:;" class="dele">×</a></td>                                                             \
                                        </tr>';
        var _diy_select_item = '<li><a href="javascript:void(0);"></a></li>';
        var shopId = window.location.search.split("=")[1];
        var allProduct = {};//所有的产品
        var isExamine, isLock;//默认的锁定状态
        initSelet();
        //初始所有品牌
        function initSelet() {
            $.when(base.getResponsData({ shopId: shopId }, "/ERPOrder/GetCustomerBrandProducttList")).done(function (res) {
                allProduct = res.data;
                isExamine = res.isExamine;
                isLock = res.isLock;
                renderBrands(res.data);
                //renderOriginalData();
            });
        }
        //填充品牌
        function renderBrands(data) {
            if (!data.length) {
                $.Message({ type: "alert", content: "该客户还未添加品牌！" });
                return;
            }
            var con = $('#brand_select ._opacity_select');
            for (var i = 0; i < data.length; i++) {
                var item = $('<option value="' + data[i].brand.id + '">' + data[i].brand.name + '</option>');
                con.append(item);
            }
        }
        //填充对应品牌产品
        function renderProducts(_list) {          
            var product = $('#pro_select').clone().removeAttr("id").show();
            var select = $("select", product);
            $("._select_text", product).text(_list[0].model_name).attr("_id", _list[0].id);
            var _select = $("._selct_items ul", product);
                for (var i = 0; i < _list.length; i++) {
                    var _option = $(_diy_select_item);
                    _option.attr("_py", pinyin.getCamelChars(_list[i].model_name).toLowerCase()).find("a").attr("_id", _list[i].id).text(_list[i].model_name);
                    _select.append(_option);
                }
            return product;
        }
       
        //填充产品数据
        function renderProductDetail(_obj, info, price) {
            $(_obj).find("td").each(function (i, o) {
                if (i == 2) {
                    $(o).text(info.format);
                    $(o).attr("model", info.produce_name);
                    $(o).attr("unit", info.unit);
                } else if (i == 3) {
                    $(o).text(info.color);
                } else if (i == 4) {
                    $(o).text(info.standard_price);
                } else if (i == 5) {
                    $(o).text(price);
                } else if (i == 7) {
                    $(o).text(info.toal_lock_num);
                } else if (i == 8) {
                    $(o).text(info.total_avaible_num);
                } else if (i == 10) {
                    $(o).find('input[name=lock_store]').eq(isLock).attr('checked','true');
                } else if (i == 11) {
                    $(o).find('input[name=check]').eq(isExamine).attr('checked', 'true');
                }
            });
        }
        //添加品牌
        $('.add_brand').on('click', function () {
            var item = $(order_item);
            var brand = $('#brand_select').clone().removeAttr("id").show();
            item.children().first().append(brand);
            $("input:eq(1)", item).bind("click", function () {
                $("#laydate").attr("id", "");
                $(this).attr("id", "laydate");
                laydate({
                    elem: '#laydate',
                    format: 'YYYY-MM-DD',
                    isclear: false,
                    istime: false,
                    istoday: false
                });
            });
            $(".dele", item).bind("click", function () {
                var _tr = $(this).parent().parent();
                $.Message({
                    type: "confirm", content: "您确定要删除该条信息?", okFun: function () {
                        _tr.remove();
                    }
                })
            });
            brand.find("select").bind("change", function () {
                var _val = $(this).val();
                item.find("input").val("");
                item.find("td").each(function (i, o) {
                    if (i == 1 || i == 2 || i == 3 || i == 4 || i == 5 || i == 7 || i == 8) {
                        $(o).empty();
                    }
                });
                if (_val != -1) {
                    for (var i = 0; i < allProduct.length; i++) {
                        if (allProduct[i].brand.id == _val && allProduct[i].product.length>0) {
                            var product_select = renderProducts(allProduct[i].product);
                            item.find("td:eq(1)").append(product_select);
                            product_select.find("a").bind("click", function () {
                                var _my_val = $(this).attr('_id');
                                item.find("input").val("");
                                item.find("td").each(function (i, o) {
                                    if (i == 2 || i == 3 || i == 4 || i == 5 || i == 7 || i == 8) {
                                        $(o).empty();
                                    }
                                });
                                if (_my_val != -1) {
                                    for (var j = 0; j < allProduct[i].product.length; j++) {
                                        if (allProduct[i].product[j].id == _my_val) {
                                            renderProductDetail(item, allProduct[i].product[j], allProduct[i].customerPrice[j]);
                                            break;
                                        }
                                    }
                                }
                            }); product_select.find("a").eq(0).trigger('click');
                            break;
                        }
                    }
                }
            });
            $('.brand_tab tbody').append(item);
        });
        //组织产品数据
        function getProductData() {
            var list = [];
            $(".brand_tab tbody tr").each(function (k, v) {
                var data = {};
                $("td", $(v)).each(function (i, o) {
                    if (i == 0) {
                        data.brand_id = $(o).find("select").val();
                        data.brand_name = $(o).find("._select_text").text();
                    }
                    else if (i == 1) {
                        data.product_id = $(o).find("._select_text").attr('_id');
                        data.product_model= $(o).find("._select_text").text();
                    }
                    else if (i == 2) {
                        data.product_format = $(o).text();
                        data.product_name = $(o).attr("model");
                        data.unit = $(o).attr("unit");
                    }
                    else if (i == 3) {
                        data.product_color = $(o).text();
                    }
                    else if (i == 5) {
                        data.product_price = $(o).text();
                    }
                    else if (i == 6) {
                        data.product_number = $(o).children().val();
                    }
                    else if (i == 9) {
                        data.factory_delivery_day = $(o).children().val();
                    }
                    else if (i == 10) {
                        data.lockStock = $(o).find("input[type='radio']:checked").parent().index();
                    }
                    else if (i == 11) {
                        data.checkFiance = $(o).find("input[type='radio']:checked").parent().index();
                    }
                });
                data.remarks = $("textarea", $(v)).val();
                list.push(data);
            });
            return list;
        }
        //组织上传数据
        function getSubmitData() {
            var data = {};
            data.shopId = shopId;
            data.shopName = $('.name').text();
            data.orderId = $(".order_num").val();
            data.factory_consumer_address = $("#address").val();
            data.factory_consumer_postcode = $("#post_code").val();
            data.factory_consumer_linkman = $("#link_man").val();
            data.factory_consumer_phone = $("#phone").val();
            data.orderInfo = getProductData();
            return data;
        }
        //检验必填写项
        function ValidateRequire(data) {
            if (data.orderId == "") {
                $.Message({ type: "alert", content: "请输入订单号！" });
                return false;
            }
            if (data.orderInfo.length == 0) {
                $.Message({ type: "alert", content: "请添加至少一条产品信息！" });
                return false;
            }
            var _is_pass = true;
            for (var i = 0; i < data.orderInfo.length; i++) {
                if (data.orderInfo[i].brand_id == -1) {
                    $.Message({ type: "alert", content: "第" + (i + 1) + "件产品没有选择品牌！" });
                    _is_pass = false;
                    break;
                }
                if (data.orderInfo[i].product_id == -1) {
                    $.Message({ type: "alert", content: "第" + (i + 1) + "件产品没有选择产品！" });
                    _is_pass = false;
                    break;
                }
                if (data.orderInfo[i].product_number == "") {
                    $.Message({ type: "alert", content: "第" + (i + 1) + "件产品没有输入数量！" });
                    _is_pass = false;
                    break;
                }
                if (data.orderInfo[i].factory_delivery_day == "") {
                    $.Message({ type: "alert", content: "第" + (i + 1) + "件产品没有输入交货日期！" });
                    _is_pass = false;
                    break;
                }
            }
            return _is_pass;
        }
        //提交
        $(".sub_mit").on("click", function () {
            var _data = getSubmitData();
            if (!ValidateRequire(_data)) {
                return;
            }
            _data.orderInfo = utils.json2str(_data.orderInfo);
            $.when(base.getResponsData(_data, "/ERPOrder/submitNewOrder", "post")).done(function (res) {
                res.code === 1 ? window.location.href = "/ERPOrder/OrderList" : $.Message({ type: "alert", content: res.message});;
            });
        });
    });
});