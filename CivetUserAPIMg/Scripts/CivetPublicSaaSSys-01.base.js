/*弹出层提示操作结果 **/
$(function () {
    if ($("#ResultMsgDiv").length == 0) {
        var thisDiv = document.createElement("div")
        thisDiv.id = "ResultMsgDiv";
        document.body.appendChild(thisDiv);
    }
    $("#ResultMsgDiv").hide();
    /***操作结果用　弹出层提示　　\\结束***************************************/

    /***弹出操作框　　开始***************************************/
    $("A[openurl]").bind("click",
        function () {
            var openurl = $(this).attr("openurl");
            var opentitle = $(this).attr("opentitle");
            var openproperty = $(this).attr("openproperty");
            if (openproperty == null) {
                _dialog({ src: openurl, title: opentitle, width: 450, height: 400, usebutton: false }).show();
            }
            else {
                var winproperty = { src: openurl, title: opentitle };
                try {
                    //读取自定的属性
                    eval("winproperty=" + openproperty + ";");
                    if (winproperty.width != null) {
                        winproperty.width = parseInt(winproperty.width, 10);
                    }
                    if (winproperty.height != null) {
                        winproperty.height = parseInt(winproperty.height, 10);
                    }
                } catch (e) {

                }
                //如果有定openurl则以openurl为准
                if (openurl != null) {
                    winproperty.src = openurl;
                }
                winproperty.src = openurl;
                if (opentitle != null) {
                    winproperty.title = opentitle;
                }
                _dialog(winproperty).show();
            }
        });

    window.page_public_dialog = null;
    window._dialog = function (obj) {
        if (obj.scrolling == null || obj.scrolling == undefined)
            obj.scrolling = "auto";
        if ($("#ifr").length == 0) {
            $("body").append("<iframe id=\"ifr\" name=\"ifr\" style=\"display:none;\" scrolling=\"" + obj.scrolling + "\" frameborder=\"0\"></iframe>");
        }
        else {
            $("#ifr").attr("scrolling", obj.scrolling);
        }

        if (obj.title == null)
            obj.title = "系统提示";
        if (obj.width == null)
            obj.width = 850;
        if (obj.height == null)
            obj.height = 600;

        var buttons = null;
        if (obj.usebutton == true) {
            buttons = {
                "确认": function () {
                    if (window.ifr.userConfirm != null) {
                        window.ifr.userConfirm();
                    }
                    $(this).dialog('close');
                },
                "取消": function () {
                    $(this).dialog('close');
                }
            }
        }

        if (obj.position == null) {
            var _result = $("#ifr").attr({ "src": obj.src }).dialog({
                title: obj.title,
                width: parseInt(obj.width, 10),
                height: parseInt(obj.height, 10),
                modal: true,
                resizable: false,
                buttons: buttons,
                close: function (event, ui) {
                    $("#ifr").remove();
                }
            });
        }
        else {
            var _result = $("#ifr").attr({ "src": obj.src }).dialog({
                title: obj.title,
                width: parseInt(obj.width, 10),
                height: parseInt(obj.height, 10),
                modal: true,
                resizable: false,
                position: obj.position,
                buttons: buttons,
                close: function (event, ui) {
                    $("#ifr").remove();
                }
            });
        }
        //debugger;
        if ((obj.height+"").indexOf('%') == -1) {
            $("#ifr").height(parseInt(obj.height, 10) - 20);
        } else {
            var dialog_h = parseInt(obj.height, 10) * 0.01 * $(window).height() - 20;
            $("#ifr").height(dialog_h);
        }
        if ((obj.width+"").indexOf('%') == -1) {
            $("#ifr").width(parseInt(obj.width, 10) - 20);
        } else {
            var dialog_w = parseInt(obj.width, 10) * 0.01 * $(window).width() - 20;
            $("#ifr").width(dialog_w);
        }
        page_public_dialog = _result;

        return _result;
    }
    window._close_dialog = function () {
        if (page_public_dialog != null) {
            page_public_dialog.dialog("close");
            page_public_dialog = null;
        }
    }
    /***弹出操作框　　\\结束***************************************/
})
function showResultMsg(data) {
    var top = 0;
    var left = ($(window).width() - $("#ResultMsgDiv").width()) / 2;
    var scrollTop = $(document).scrollTop() + 10;
    var scrollLeft = $(document).scrollLeft();
    $("#ResultMsgDiv").css({ position: 'absolute', top: top + scrollTop, left: left + scrollLeft });

    //顯示提示的總時間
    var totaltime = (data.Time == undefined ? 2000 : data.Time);
    if (data.Result != undefined) {
        if (data.Result == 1) {  //操作成功
            $("#ResultMsgDiv").css({ "background-color": "#008800" });
            if (data.Message == undefined) data.Message = "操作成功";
        }
        else if (data.Result == 0) {  //操作失败
            $("#ResultMsgDiv").css({ "background-color": "#FF8800" });
            if (data.Message == undefined) data.Message = "操作失败";
        }
        else if (data.Result == -1) {  //操作失败
            if (data.Message == undefined) data.Message = "系统异常";
        }

        $("#ResultMsgDiv").html(data.Message).fadeIn(totaltime / 3);
        $("#ResultMsgDiv").fadeOut(totaltime * 2 / 3);

        if (data.Redirect != undefined) {
            window.setTimeout("window.location.href = '" + data.Redirect + "'", totaltime);
        }
        if (data.PRedirect != undefined) {
            window.setTimeout("window.parent.location.href = '" + data.PRedirect + "'", totaltime);
        }
        if (data.Refresh != undefined) {
            window.setTimeout("window.parent.location.reload();", totaltime);
        }
    }
}
/*******手机模拟器   开始*********************************************************************************************************/
function showSimulator(url) {
    $("#Simulator-Content").html("<iframe id=\"contentframe\" name=\"contentframe\" frameborder=\"0\" src=\"" + url + "\" style=\"width: 340px; height: 540px; margin: 0px auto;\" scrolling=\"no\"></iframe>");
    $("#Simulator .ui-widget-overlay").height($(document).height());
      
    $("#Simulator-obj").each(function () { this.style.top = (document.body.scrollTop + 50) + "px"; });
    $("#Simulator").show();
}
function closeSimulator() {
    $("#Simulator-Content").html("");
    $("#Simulator").hide();
}
/*******手机模拟器   结束*********************************************************************************************************/

//获得IE的版本
function getIEVersion() {
    var agent = navigator.userAgent;
    var index = agent.indexOf("MSIE");
    if (index != -1) {
        return parseFloat(agent.substring(index + 4, index + 10).split(';')[0]);
    }
    return "-1";
}

//刷新a标签text
function RefreshText(id, obj) {
    if (obj.RefreshText != undefined && $("#" + id) != undefined) {
        $("#" + id).text(obj.RefreshText);
    }
}