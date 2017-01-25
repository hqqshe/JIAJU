require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'laydate', 'utils', 'domReady'], function ($, _, base, _, utils, _) {
        base.init();
        var order_item = ' <tr class="select">                                                                                                                               \
                                            <td></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                            <td></td>                                                                                                                                                 \
                                            <td><input type="text" placeholder="请填写数量" onkeyup="this.value = this.value.replace(/\D/g, \'\')" onafterpaste="    this.value = this.value.replace(/\D/g, \'\')"></td>\
                                            <td></td>                                                                                                                                                \
                                            <td></td>                                                                                                                                                \
                                            <td><input type="text" placeholder="请选择交货日期"></td>                                    \
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
        var sarr = window.location.search.split("&");
        var shopId = sarr[1].split('=')[1];
        var orderId = sarr[0].split('=')[1];
        var allProduct = {};//所有的产品
        var isExamine, isLock;//默认的锁定状态
        var delete_id = 0;
        initSelet();
        //初始所有品牌
        function initSelet() {
            $.when(base.getResponsData({ shopId: shopId }, "/ERPOrder/GetCustomerBrandProducttList")).done(function (res) {
                allProduct = res.data;
                isExamine = res.isExamine;
                isLock = res.isLock;
                renderBrands(res.data);
                renderOriginalData();
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
        function renderProducts(list) {
            var product = $('#pro_select').clone().removeAttr("id").show();
            var select = $("select", product);
            for (var i = 0; i < list.length; i++) {
                var item = $('<option value="' + list[i].id + '">' + list[i].model_name + '</option>');
                select.append(item);
            }
            return product;
        }
        //渲染原产品数据
        function renderOriginalData() {
            var _my_brand_id = $('._original_data ._opacity_select').eq(0).attr('_val');
            var _my_product_id =$('._original_data ._opacity_select').eq(1).attr('_val');
            var _brand_select_obj = $("._original_data select:eq(0)");
            var _product_select_obj = $("._original_data select:eq(1)");
            //渲染原订单产品数据
            for (var i = 0; i < allProduct.length; i++) {
                var item = $('<option value="' + allProduct[i].brand.id + '">' + allProduct[i].brand.name + '</option>');
                if (_my_brand_id == allProduct[i].brand.id) {
                    item.attr("selected", "selected");
                    for (var j = 0; j < allProduct[i].product.length; j++) {
                        var item_2 = $('<option value="' + allProduct[i].product[j].id + '">' + allProduct[i].product[j].model_name + '</option>');
                        if (allProduct[i].product[j].id == _my_product_id) {
                            item_2.attr("selected", "selected");
                        }
                        _product_select_obj.append(item_2);
                    }
                }
                _brand_select_obj.append(item);
            }
            $("._original_data input:eq(1)").bind("click", function () {
                $("#laydate").attr("id", "");
                $(this).attr("id", "laydate");
                laydate({
                    elem: '#laydate',
                    format: 'YYYY-MM-DD',
                    isclear: false,
                    istime: false,
                    istoday: false,
                    choose: function (dates) { //选择好日期的回调
                    }
                });
            });
            var _original_tr = $("._original_data");
            //品牌选择绑定事件
            _brand_select_obj.bind("change", function () {
                var _val = $(this).val();
                _original_tr.find("input").val("");
                _original_tr.find("td").each(function (i, o) {
                    if (i == 1 || i == 2 || i == 3 || i == 4 || i == 5 || i == 7 || i == 8) {
                        $(o).empty();
                    }
                });
                if (_val != -1) {
                    for (var i = 0; i < allProduct.length; i++) {
                        if (allProduct[i].brand.id == _val) {
                            var product_select = renderProducts(allProduct[i].product);
                            _original_tr.find("td:eq(1)").append(product_select);
                            product_select.find("select").bind("change", function () {
                                var _my_val = $(this).val();
                                _original_tr.find("input").val("");
                                _original_tr.find("td").each(function (i, o) {
                                    if (i == 2 || i == 3 || i == 4 || i == 5 || i == 7 || i == 8) {
                                        $(o).empty();
                                    }
                                });
                                if (_my_val != -1) {
                                    for (var j = 0; j < allProduct[i].product.length; j++) {
                                        if (allProduct[i].product[j].id == _my_val) {
                                            renderProductDetail(_original_tr, allProduct[i].product[j], allProduct[i].customerPrice[j]);
                                            break;
                                        }
                                    }
                                }
                            });
                            break;
                        }
                    }
                }
            });
            //产品选择绑定事件
            _product_select_obj.bind("change", function () {
                var _my_val = $(this).val();
                _original_tr.find("input").val("");
                _original_tr.find("td").each(function (i, o) {
                    if (i == 2 || i == 3 || i == 4 || i == 5 || i == 7 || i == 8) {
                        $(o).empty();
                    }
                });
                if (_my_val != -1) {
                    for (var i = 0; i < allProduct.length; i++) {
                        if (allProduct[i].brand.id == _my_brand_id) {
                            for (var j = 0; j < allProduct[i].product.length; j++) {
                                if (allProduct[i].product[j].id == _my_val) {
                                    renderProductDetail(_original_tr, allProduct[i].product[j], allProduct[i].customerPrice[j]);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            });
            //绑定删除事件
            $(".dele", _original_tr).bind("click", function () {
                var _tr = $(this).parent().parent();
                $.Message({
                    type: "confirm", content: "您确定要删除该条信息?", okFun: function () {
                        delete_id = _tr.attr("_id");
                        _tr.remove();
                    }
                })
            });
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
                        if (allProduct[i].brand.id == _val) {
                            var product_select = renderProducts(allProduct[i].product);
                            item.find("td:eq(1)").append(product_select);
                            product_select.find("select").bind("change", function () {
                                var _my_val = $(this).val();
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
                            });
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
                if (typeof $(v).attr("_id") != "undefined") {
                    data.id = $(v).attr("_id");
                } else {
                    data.id = 0;
                }
                $("td", $(v)).each(function (i, o) {
                    if (i == 0) {
                        data.brand_id = $(o).find("select").val();
                        data.brand_name = $(o).find("._select_text").text();
                    }
                    else if (i == 1) {
                        data.product_id = $(o).find("select").val();
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
                    else if (i == 10) {
                        data.lockStock = $(o).find("input[type='radio']:checked").parent().index();
                    }
                    else if (i == 11) {
                        data.checkFiance = $(o).find("input[type='radio']:checked").parent().index();
                    }
                });
                $("input", $(v)).each(function (i, o) {
                    if (i == 0) {
                        data.product_number = $(o).val();
                    } else if (i == 1) {
                        data.factory_delivery_day = $(o).val();
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
            data.deleteId = delete_id;
            data.shopId = shopId;
            data.shopName = $('._input_panel .name').text();
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
                $.Message({ type: "alert", content: "请输入订单信息！" });
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
            $.when(base.getCodeData(_data, "/ERPOrder/editOrder", "post")).done(function () {
                window.location.href = "/ERPOrder/OrderList";
            });
        });
    });
});