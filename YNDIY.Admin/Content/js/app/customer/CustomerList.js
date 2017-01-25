require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'uploadForm', 'domReady'], function ($, _, base, utils, _) {
        base.init();
        //导入客户
        $("._upload").uploadForm({
            type: "customer_excel",
            success: function (_result) {
                eval("var response =" + _result);
                $.Message({ type: "alert", content: response.message });
                if (response.code) {
                    window.location.reload();
                }
            }
        });
        //加载数据
        window._load_items = function (_page_index) {
           $.when(base.loadHtmlData({ type: $("._opacity_select").val(), searchKey: $(".search_input").val(), pageIndex: _page_index }, "/ERPCustomer/GetCustomerList")
            ).done(function (res) {$("._ajax_container").empty().append(res);});
        }
        _load_items(1);
        //绑定加载数据事件 begin
        $("._opacity_select").live("change", function () { _load_items(1); });
        //删除客户
        $("._ajax_container").on("click", ".dele", function () {
            var _that = $(this);
            $.Message({
                type: "confirm", content: "你确定删除此客户吗,删除后该客户下面的所有内容将会清除！", okFun: function () {
                    $.when(base.getCodeData({ id: _that.attr("_id") }, "/ERPCustomer/deleteCustomer")).done(function () {
                        $.Message({ type: "alert", content: "删除成功！" });
                        _load_items(1);
                    });
                }
            });
        });
    });
});