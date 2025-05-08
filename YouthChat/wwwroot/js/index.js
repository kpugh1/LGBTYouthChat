const imgBOT = "/images/head.png";
const imgPerson = "/images/user.png";
const nameBOT = "assistent";
const namePerson = "user";

$(function () {
    let $Chat = $(".msger-chat");
    let $Loader = $('.imgLoader');
    var messages = [];
    
    addChatMessage(nameBOT, imgBOT, "left", "Hey! How may I help you?");

    $('#sendButton').click(function () {
        var message = $('#messageInput').val();
        askChatGPT(message);
        $('#messageInput').val('');
        return false;
    });

    function askChatGPT(message) {
        // console.log(messages);
        console.log(JSON.stringify({"messages": messages}) );
        $.ajax({
                url: `/Chat?UserResponse=${message}`,
                type: 'POST',
                data:  JSON.stringify({"messages": messages}),
                async: true,
                contentType: 'application/json',
                success: function (response) {
                        addChatMessage(nameBOT, imgBOT, "left", response.data);
                        $Loader.hide();
                    }
                });
        addChatMessage(namePerson, imgPerson, "right", message);
    }

    function addChatMessage(name, img, side, text) {
        messages.push({"role": name,  "content": text});
        const msgHTML = `
                    <div class="msg ${side}-msg">
                        <div class="msg-img" style="background-image: url(${img})"></div>

                        <div class="msg-bubble">
                        <div class="msg-info">
                            <div class="msg-info-name">${name}</div>
                            <div class="msg-info-time">${formatDate(new Date())}</div>
                        </div>

                        <div class="msg-text">${text}</div>
                        </div>
                    </div>
                    `;

        $Chat.append($(msgHTML));

        if (side == "left") {
            var loaderHTML = `<div id="dvLoader"><img class="imgLoader" src="/images/loader.gif" /></div>`;
            $Chat.append($(loaderHTML));
        }

        $Chat.scrollTop($Chat.scrollTop() + 500);

        return false;
    }

    function formatDate(date) {
        var dateHour = date.getHours();
        var dateMin = date.getMinutes();
        var h;
        var timeOfday;
        if(dateHour == "00")
        {
            h = 12;
            timeOfday = "AM";
        }
        else if(dateHour > 12)
        {
            h = dateHour - 12;
            timeOfday = "PM";
        }
        else
        {
            h = dateHour
            timeOfday = "AM";
        }
        
        const m = dateMin < 10 ?  '0' + date.getMinutes() : dateMin;

        return `${h}:${m} ${timeOfday}`;
    }
});