(function ($) {
    $.fn.uploadForm = function (_options) {
        var _upload_url = ["/Upload/UploadImage", "/Upload/UploadModel", "/Upload/UploadFile", "/Upload/ExcelTemplate", "/Upload/CustomerExcelTemplate", "/Upload/SupplierExcelTemplate", "/Upload/ProcedureExcelTemplate"];
        var _settings = {
            type: "image",//image:图片  model:3D文件模型  file:文本描述文件
            success: function (_return_url) {
                console.log(_return_url);
            }
        };
        _settings = $.extend({}, _settings, _options);
        var _upload_html = '<div id="_upload_plugin_form" style="position:fixed;top:0;right:0;bottom:0;left:0;">                                                                                                                                    \
                            <div style="position: absolute; top: 0; right: 0; bottom: 0; left: 0; background-color: #000; opacity: .5; filter: alpha(opacity=50);"></div>                                                                       \
                            <div style="position:absolute;width:300px;height:140px;top:50%;left:50%;margin-left:-150px;margin-top:-70px;background-color:#FFF;">                                                                                \
                                <div style="position:absolute;height:40px;border-bottom:1px solid #ccc;width:300px;text-align:center;line-height:40px;">                                                                                        \
                                    <span style="color:#2b2b2b;font-size:14px">上传</span>                                                                                                                                                      \
                                    <a href="javascript:void(0);" class="_close_upload_window" style="position:absolute;display:block;width:40px;height:40px;text-align:center;text-decoration:none;font-size:30px;line-height:40px;top:0;right:0;">                         \
                                        <span style="color:#c65b5b;">×</span>                                                                                                                                                                  \
                                    </a>                                                                                                                                                                                                        \
                                </div>                                                                                                                                                                                                          \
                                <iframe _load_times="0" name="_upload_file_form" style="width: 300px; height: 100px; border: none; *border: 0; border-width: 0; position: absolute; top: 41px; left: 0;" frameborder="0" src=""></iframe>    \
                            </div>                                                                                                                                                                                                              \
                        </div>';
        $(this).each(function (i, o) {
            $(o).bind("click", function () {
                //上传窗口
                var _up_form = $(_upload_html);
                //关闭上传窗口
                var _close_btn = $("._close_upload_window", _up_form);
                _close_btn.bind("click", function () {
                    _up_form.remove();
                });
                //绑定上传地址
                var _iframe = $("iframe", _up_form);
                _iframe.attr("src", _settings.type == "image" ? _upload_url[0] :
                                    _settings.type == "model" ? _upload_url[1] :
                                    _settings.type == "file" ? _upload_url[2] :
                                    _settings.type == "material_excel" ? _upload_url[3] :
                                    _settings.type == "customer_excel" ? _upload_url[4] :
                                    _settings.type == "supplier_excel" ? _upload_url[5] :
                                    _settings.type == "procedure_excel" ? _upload_url[6] : "");
                $("._loading").show();
                //上传页面加载完成事件
                _iframe.bind("load", function () {
                    $("._loading").hide();
                    var _load_times = $(this).attr("_load_times");
                    if (_load_times == "0") {//上传页面加载完成
                        _iframe.attr("_load_times", "1");
                    } else {//上传完成事件
                        _settings.success($(window.frames["_upload_file_form"].document).text());
                        _close_btn.click();
                    }
                });
                $("body").append(_up_form);
            });
        });
    }
}(jQuery));