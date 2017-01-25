require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'base', 'utils', 'pop_window', 'domReady'], function ($, base,utils) {
        base.init();
        var order_id;
        var pop_item = '<tr><td><input type="checkbox"></td><td>113</td><td>213</td><td class="_td_10 _over_text"><input type="text" class="_input_area" placeholder="本次入库数量" onkeyup="this.value = this.value.replace(/\\D/g, \'\')" onafterpaste="this.value = this.value.replace(/\\D/g, \'\')"></td></tr>';
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
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ store_state: $(".store_status").val(), brandId: $(".brandId").val(), searchKey: $('.search_input').val(), pageIndex: _page_index }, "/ERPStore/GetProductStoreIn")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        //入库面板
        $("._ajax_container").on("click", ".store_in", function () {
            order_id = $(this).attr("_id");
            var _data = $(this).parent().prev().prev().attr('title');
            if (_data) {
                _data = _data.replace(/【/g, '').split('】');
                fillPop(_data);
                $('.plan_con').fadeIn();
            } else {
                $.Message({ type: "alert", content: "参数错误,请刷新页面！" });
            }
        });
        //填充面板
        function fillPop(_data) {
            var con = $('.plan_con ._list_table tbody');
            con.empty();
            for (var i = 0; i < _data.length; i++) {
                if (!_data[i]) continue;
                var item = $(pop_item);
                var ar = _data[i].split('：');
                item.children().eq(1).text(ar[0]);
                item.children().eq(2).text(ar[1]);
                con.append(item);
            }
        }
        //默认输入
        $(".plan_con input:first").on("blur", function () {
            var _input = +$(this).val();
            $(".plan_con ._list_table input").each(function (k, v) {
                //var _val = +$(v).val();
                //_val < _input && $(v).val(_input);
                $(v).val(_input);
            });
        });
        //提交入库
        $(".plan_con").on("click", ".sub_mit", function () {
            var _data = {};
            var arr = [];
            var isok = false;
            $(".plan_con ._list_table .plus").each(function (k, v) {
                var part = {};
                part.packageNumber = $(v).children().eq(1).text();
                part.number = $(v).children().eq(3).children().val();
                if (!part.number) {
                    $.Message({ type: "alert", content: "请输入" + part.packageNumber + "本次入库数量！" });
                    isok = true;
                    return false;
                }
                arr.push(part);
            });
            if (isok) { return }
            if (arr.length === 0) { $.Message({ type: "alert", content: "请选择包！" }); return; }
            _data.orderId = order_id;
            _data.data = utils.json2str(arr);
            $.when(base.getCodeData(_data, "/ERPStore/SetStorageIn","post")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.plan_con').fadeOut()
                _load_items(1);
            });
        });
    });
});