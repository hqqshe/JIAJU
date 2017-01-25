require(['/Content/minjs/config.js'], function () {
    require(['jquery', 'pop_window', 'base', 'laydate', 'utils', 'domReady'], function ($, _, base, _, utils, _) {
        base.init();
        $(".sub_mit").live("click", function () {
            if (!$("input[name='account']").val()) {
                $.Message({ type: "alert", content: "账号不能为空！" });
                return;
            }
            if (!$("input[name='password']").val()) {
                $.Message({ type: "alert", content: "密码不能为空！" });
                return;
            }
            if ($("input[name='password']").val() != $("input[name='repassword']").val()) {
                $.Message({ type: "alert", content: "两次密码输入不一致！" });
                return;
            }
            if (!$("input[name='nick_name']").val()) {
                $.Message({ type: "alert", content: "姓名不能为空！" });
                return;
            }
            var employeeNo = $.trim($("input[name='employee_no']").val());
            if (!employeeNo) {
                $.Message({ type: "alert", content: "工号不能为空！" });
                return;
            }
            if (!/^[a-zA-Z0-9]{1,6}$/.test(employeeNo)) {
                $.Message({ type: "alert", content: "工号只能是字母数字,最长不能超过6位！" });
                return;
            }
            if (!$("select[name='factory_department_id']").val()) {
                $.Message({ type: "alert", content: "请选择生产线！" });
                return;
            }
            $.when(base.getCodeData($("form[name='employer']").serialize(), "/ERPemployer/saveEmployer", "post")).done(function () {
                window.location.href = "/ERPemployer/EmployerList";
            });
        });
    });
});