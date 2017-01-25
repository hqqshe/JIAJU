require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'pop_window', 'domReady'], function ($, base) {
        base.init();
        var order_id;
        var pro_item = ' <tr>                                                                                                                 \
                                        <td><input type="radio" name="pro"></td>                                 \
                                        <td></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                        <td class="_td_10 _over_text"></td>                                                \
                                        <td></td>                                                                                                      \
                                        <td ></td>                                                                                                     \
                                        <td></td>                                                                                                      \
                                        <td></td>                                                                                                      \
                                        <td><input type="text" class="_input_area" placeholder="本次入库数量" onkeyup="this.value = this.value.replace(/\D/g, \'\')" onafterpaste="this.value = this.value.replace(/\D/g, \'\')"></td>\
                                    </tr>';
        //加载数据
        window._load_items = function (_page_index) {
            $.when(base.loadHtmlData({ brandId: $(".brandId").val(), searchKey: $('.search_input').val(), pageIndex: _page_index }, "/ERPStore/GetProductStore")
             ).done(function (res) { $("._ajax_container").empty().append(res); });
        }
        initSelet();
        //初始所有品牌
        function initSelet() {
            $.when(base.getData({}, "/ERPBrand/GetAllBranList")).done(function (res) {
                fillSelect(res);
                _load_items(1);
            });
        }
        //初始所有品牌 填充
        function fillSelect(data) {
            var con = $('.brandId')
            con.empty();
            con.prev().find('._select_text').text('全部');
            con.append($('<option value="-1">全部</option>'));
            for (var i = 0; i < data.length; i++) {
                var item = $('<option value="1"></option>');
                item.attr("value", data[i].id).text(data[i].name);
                con.append(item);
            }
            //con.val(data[0].id).prev().find('._select_text').text(data[0].name);
        }
        //盘点库存面板
        $("._ajax_container").on("click", ".allot", function () {
            order_id = $(this).attr("_id");
            var tds = $(this).parents('tr').eq(0).children();
            var con = $('.allot_con');
            $('._list_table tbody td', con).each(function (k, v) {
                if (k === 6) {
                    $(v).text(tds.eq(k + 3).text())
                } else if (k === 7) {
                    return false;
                } else {
                    $(v).text(tds.eq(k).text()).attr('title', tds.eq(k).text())
                }
            });
            $('._list_table:eq(0) tbody td', con).each(function (k, v) {
                $(v).text(tds.eq(k).text())
            });
            con.fadeIn().find('input').val('');
            con.find('textarea').val('');
        });
        //提交盘点
        $(".allot_con").on("click", ".sub_mit", function () {
            var num = $('.allot_con input').eq(0).val();
            if (num === ''||isNaN(num) || num < 0) {
                $.Message({ type: "alert", content: "请输入盘点数量！" });
                return;
            }
            var tip = $('.allot_con tip_txt').val();
            $.when(base.getCodeData({ productId: order_id, packageNumber: $('.allot_con .bag_num').text(), inventoryCount: num, tip: tip }, "/ERPStore/SetInventory")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.allot_con').fadeOut()
                _load_items($('._fontColor').text());
            });
        });
        //调拨库存面板
        $("._ajax_container").on("click", ".edit_store", function () {
            order_id = $(this).attr("_id");
            $('.plan_con').fadeIn();
        });
        //查询关联产品
        $(".plan_con .search_btn").on("click", function () {
            var _val = $('.plan_con .search_input').val();
            if (!_val) {
                $.Message({ type: "alert", content: "请输入产品型号！" });
                return;
            }
            $.when(base.getData({ searchKey: _val }, "/ERPBrand/GetRelationProducts")).done(function (res) {
                fillPro(res);
            });
        });
        //填充产品
        function fillPro(data) {
            var con = $('.plan_con .pro_tab tbody');
            con.empty();
            if (data.length === 0) return;
            for (var i = 0; i < data.length; i++) {
                var item = $(pro_item);
                item.children().eq(0).attr("_id", data[i].id);
                item.children().eq(1).text(data[i].produce_name);
                item.children().eq(2).text(data[i].model_name);
                item.children().eq(3).text(data[i].format).attr('title', data[i].format);
                item.children().eq(4).text(data[i].color);
                item.children().eq(5).text(data[i].unit);
                item.children().eq(6).text(data[i].standard_price);
                con.append(item);
            }
        }
        //提交入库
        $(".plan_con").on("click", ".sub_mit", function () {
            var num = parseInt($('.plan_con input').eq(0).val());
            if (!num) {
                $.Message({ type: "alert", content: "请输入本次入库数量！" });
                return;
            }
            var date = $('.plan_con input').eq(1).val();
            if (!date) {
                $.Message({ type: "alert", content: "请选择出库日期！" });
                return;
            }
            var tip = $('.plan_con tip_txt').val();
            $.when(base.getCodeData({ order_id: order_id, num: num, date: date, tip: tip }, "/ERPBrand/DeleteProduct")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.plan_con').fadeOut()
                _load_items($('._fontColor').text());
            });
        });
    });
});