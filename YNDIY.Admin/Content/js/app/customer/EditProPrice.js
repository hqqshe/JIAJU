require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'utils', 'domReady'], function ($, _, base, utils, _) {
        base.init();
        var proprice_item = ' <tr>                                                                                                                                                                                                                                                                                                                                                                                                               \
                                                <td class="_td_10 _over_text">产品名称</td>\
                                                <td class="_td_10 _over_text">产品型号</td>\
                                                <td class="_td_10 _over_text">产品规格</td>\
                                                <td>产品颜色</td>\
                                                <td>单位</td>\
                                                <td>标准单价（元）</td>\
                                                <td><input type="text" placeholder="客户单价" class="_input_area" onkeyup="this.value = this.value.replace(/[^0-9^.]/g, \'\')" onafterpaste="this.value = this.value.replace(/[^0-9]/g,\'\')"></td>      \
                                            </tr>';
        var cid = window.location.href.split('=')[1];
        var big_data = null;
        var brand_id;
        $(".pro_price_tab ").on("blur", "tbody input", function () {
            var _price = {};
            _price.id = typeof $(this).attr('_id') == "undefined" ? 0 : $(this).attr('_id');
            _price.price = parseInt($(this).parents('tr').eq(0).children().last().children().val());
            _price.price = _price.price ? _price.price : 0;
            var index = $(this).parents('tr').eq(0).index();
            big_data[index].price = _price;
        });
        //删除品牌
        $('.brand_tab').on('click', '.dele', function () {
            var _that = $(this);
            $.Message({
                type: "confirm", content: "你确定删除此产品吗?删除后该产品下面的所有内容将会清除！", okFun: function () {
                    $.when(base.getCodeData({ customer_id: cid, brand_id: _that.attr("_id") }, "/ERPBrand/DeleteSomeoneBrandInCustomer")).done(function () {
                        window.location.reload();
                    });
                }
            });
        });
        //编辑产品价格面板
        $('.brand_tab').on('click', '.edit_price', function () {
            brand_id = $(this).attr("_id");
            $.when(base.getData({ customer_id: cid, brand_id: brand_id }, "/ERPBrand/GetAllPriceByBrandId")).done(function (res) {
                big_data = res;
                for (var i = 0; i < big_data.length; i++) {
                    delete big_data[i].product.create_time;
                    delete big_data[i].product.modify_time;
                    delete big_data[i].product.package_info;
                    delete big_data[i].product.parts_info;
                    delete big_data[i].product.packing_info;
                    delete big_data[i].product.process_info;
                    delete big_data[i].product.try_process_info;
                    if (big_data[i].price != null) {
                        delete big_data[i].price.create_time;
                        delete big_data[i].price.modify_time;
                    }
                }
                fillProPrice(big_data);
                $('.pro_price_con').fadeIn();
            });
        });
        //编辑产品价格面板 填充
        function fillProPrice(data) {
            var con = $('.pro_price_tab tbody');
            con.empty();
            for (var i = 0; i < data.length; i++) {
                var item = $(proprice_item)
                item.children().eq(0).text(data[i].product.produce_name).attr("title", data[i].product.produce_name);
                item.children().eq(1).text(data[i].product.model_name).attr("title", data[i].product.model_name);
                item.children().eq(2).text(data[i].product.format).attr("title", data[i].product.format);
                item.children().eq(3).text(data[i].product.color);
                item.children().eq(4).text(data[i].product.unit);
                item.children().eq(5).text(data[i].product.standard_price);
                item.children().eq(6).children().val(data[i].product.standard_price);
                if (data[i].price != null) {
                    item.children().eq(6).children().val(data[i].price.price).attr("_id", data[i].price.id);
                } else {
                    item.children().eq(6).children().val(data[i].product.standard_price).attr("_id", "0");
                }
                con.append(item);
            }
        }
        //编辑产品价格面板 提交
        $(".sub_mit").on("click", function () {
            var data_list = [];
            $('.pro_price_tab tbody tr').each(function (k, v) {
                var num1 = $(v).children().eq(5).text();
                var num2 = $(v).children().eq(6).children().val();
                if (num1 != num2) {
                    data_list.push(big_data[k]);
                }
            });
            $.when(base.getCodeData({ brand_id: brand_id, customer_id: cid, data: utils.json2str(data_list) }, "/ERPBrand/SetPriceInBrand", "post")).done(function () {
                $.Message({ type: "alert", content: "编辑成功！" });
                $('.pro_price_con').fadeOut();
            });
        });
    });
});