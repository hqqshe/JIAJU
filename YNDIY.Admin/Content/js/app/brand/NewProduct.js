require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'uploadForm', 'domReady'], function ($, _, base, _, _) {
        base.init();
        var _show_img = null;
        var pid = window.location.href.split('=')[1];
        $(".upload_img").on("click", function () {
            _show_img = $(this).prev().find("img");
        }).uploadForm({
            success: function (_result_str) {
                if (/.png$/.test(_result_str)) {
                    _show_img.attr("src", _result_str).show();
                } else {
                    if ("sizelimit" == _result_str) {
                        alert("图片过大！");
                        $.Message({ type: "alert", content: response.message });
                    } else {
                        alert("上传失败！");
                        $.Message({ type: "alert", content: response.message });
                    }
                }
            }
        });
        //获取输入产品信息
        function GetInfoItems() {
            var _items_data = {};
            _items_data.image = $(".img_view img").attr("src");
            _items_data.brandId = $("#brand_select").val();
            _items_data.unit = $("._search_filter .unit_select").val();
            $("._input_panel input").each(function (i, o) {
                i == 0 ? _items_data.model = $(o).val() :
                i == 1 ? _items_data.name = $(o).val() :
                i == 2 ? _items_data.format = $(o).val() :
                i == 3 ? _items_data.color = $(o).val() :
                i == 4 ? _items_data.price = $(o).val() :
                i == 5 ? _items_data.lowerLine = $(o).val() :
                i == 6 ? _items_data.remark = $(o).val() : null;
            });
            return _items_data;
        }
        //检验必填写项
        function validate(_data) {
            if (_data.model == "") {
                $.Message({ type: "alert", content: "产品型号不能为空！" });
                return true;
            }
            if (_data.name == "") {
                $.Message({ type: "alert", content: "产品名称不能为空！" });
                return true;
            }
            if (_data.format == "") {
                $.Message({ type: "alert", content: "产品规格不能为空！" });
                return true;
            }
            if (_data.color == "") {
                $.Message({ type: "alert", content: "产品颜色不能为空！" });
                return true;
            }
            if (_data.price == "") {
                $.Message({ type: "alert", content: "产品标准单价不能为空！" });
                return true;
            }
            if (_data.lowerLine == "") {
                $.Message({ type: "alert", content: "库存下限不能为空！" });
                return true;
            }
            return false;
        }
        //提交
        $(".sub_mit").on("click", function () {
            var _data = GetInfoItems();
            if (validate(_data)) {
                return;
            }
            _data.id = pid ? pid : null;
            if (pid) {
                $.when(base.getCodeData(_data, "/ERPBrand/SaveEditProduct")).done(function () { window.location.href = "/ERPBrand/ProductList"; });
            } else {
                $.when(base.getCodeData(_data, "/ERPBrand/AddProduct")).done(function () { window.location.href = "/ERPBrand/ProductList"; });
            }
        });
    });
});
