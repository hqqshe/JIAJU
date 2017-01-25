require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'domReady'], function ($, _, base, _, _) {
        base.init();
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ searchKey: $('.search_input').val(), pageIndex: _page_index }, "/ERPBrand/GetBrandList")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //新建品牌面板
        $("._btns").on("click", "a", function () {
            $('.new_brand_con').fadeIn().find(".sub_num").val("").parent().find(".tip_txt").val("");
        });
        //提交新建品牌
        $(".new_brand_con").on("click", ".sub_mit", function () {
            var _that = $(this);
            var _val = $('.new_brand_con .sub_num').val();
            if (!_val) {
                $.Message({ type: "alert", content: "请输入品牌名称！" });
                return;
            }
            $.when(base.getCodeData({ 'brandName': _val, 'remarks': $('.new_brand_con .tip_txt').val() }, "/ERPBrand/AddBrand")).done(function () {
                $.Message({ type: "alert", content: "提交成功！" });
                $('.new_brand_con').fadeOut();
                _load_items(1);
            });
        });
        //删除品牌
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            $.Message({
                type: "confirm", content: "确定删除该品牌吗？删除后，相关数据也将被删除，不可恢复，请慎重！", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPBrand/DeleteBrand")).done(function () {
                        $.Message({ type: "alert", content: "删除成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});
