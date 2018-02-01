$(document).ready(function () {
    var base_url = "../";
    var path = window.location.pathname.toLowerCase(); // Get urn we are visiting
    var pathArr = path.replace(/^\//, "").split("/");  // Split every path
    var page = pathArr[pathArr.length - 1]; // Set page to last path part
    
    // Click trigger when a link is clicked
    $("a").click(function (event) {
        // Check if it's a square link
        if ($(this).children(".square").length > 0) {
            event.preventDefault(); // Prevent browser to load page

            // Check if it's your turn, by data attribute turn on yourboard
            if ($("#yourboard").attr("data-turn") == "true") {
                // Get clicked square
                var square = $(this).children(".square");

                // Check if position is register hit before
                if (!square.hasClass("hit") && !square.hasClass("boatHit")) {
                    // Get position from data-attributes x and y on square
                    var positionX = square.attr("data-x");
                    var positionY = square.attr("data-y");

                    // Get url from link
                    var url = $(this).attr("href");

                    // Make a GET Ajax-call (javascript pageload in background)
                    $.get(url, function (data) {
                        // Check and takecare turn changes and if game is over redirect to GameEnd page
                        CheckGameMeta(data);

                        // Set class for hit on square, if we got any response about it
                        if (data.result != undefined) {
                            if (data.result) {
                                square.addClass("boatHit");
                            }
                            else {
                                square.addClass("hit");
                            }
                        }

                        // If got any errors show them
                        if (data.Messages != undefined) {
                            $.each(data.Messages, function (messageType, messages) {
                                InsertAlert(messages, messageType);
                            });
                        }

                    }, "json") // Ask for JSON response
                    .fail(function (xhr, statusText) {
                        // If it fail, redirect by status code response
                        FailLoadDoRedirect(xhr.status, url);
                    });
                }
                // If position is already hit, show error about it
                else {
                    InsertAlert(["Position is already hit!"], "danger");
                }
            }
            // If it is not our turn, show error about it
            else {
                InsertAlert(["Not your turn!"], "danger");
            }
        }
    });

    // Update game by GET Ajax-call (javascript pageload in background)
    function UpdateGame() {
        // Get lastUpdateElement
        if (page == "waiting") {
            var lastUpdateElement = $(".waiting-bg");
        }
        else {
            var lastUpdateElement = $("#playerboards");
        }
        
        // Get lastUpdate from data-attribute lastupdate
        var lastUpdate = lastUpdateElement.attr("data-lastupdate");

        $.post("UpdateGame", { lastupdate: lastUpdate }, function (data) {
            // Set game last update, for later use
            lastUpdateElement.attr("data-lastupdate", data.lastupdate);

            // Check if game is started
            if (data.gamestart) {
                // If we are on waiting view redirect us to Game view
                if (page == "waiting") {
                    window.location.href = "Game";
                }
                else {
                    // Check and takecare turn changes and if game is over redirect to GameEnd page
                    CheckGameMeta(data);

                    // Check and takecare of boards changes
                    UpdateBoards(data.boards);
                }
            }
            // If not redirect if we are on Game view
            else if (page == "game") {
                window.location.href = "Waiting";
            }
        }, "json") // Request JSON-response
        .fail(function (xhr, statusText) {
            // If it fail, redirect by status code response
            FailLoadDoRedirect(xhr.status, "");
        });
    }

    // Set update interval of game updates to 5 sec
    setInterval(UpdateGame, 5000);

    // Check and takecare of turn changes and redirect if game is ended
    function CheckGameMeta(data) {
        // If game is ended, redirect to GameEnd view
        if (data.gameend != undefined && data.gameend) {
            window.location.href = "GameEnd";
        }

        // Check who turn it's, and set that with data-attributes
        if (data.turn.toString() == $("#yourboard").attr("data-playerid")) {
            $("#yourboard").attr("data-turn", "true");
            $("#enemyboard").attr("data-turn", "false");
        }
        else {
            $("#yourboard").attr("data-turn", "false");
            $("#enemyboard").attr("data-turn", "true");
        }
    }

    // Redirect by response status code
    function FailLoadDoRedirect(statusCode, defaultRedirect) {
        defaultRedirect = (defaultRedirect == null || defaultRedirect == undefined) ? base_url : defaultRedirect; // If we not got any default redirect url, then redirect to base_url

        // 401 redirect to base_url
        if (statusCode == 401) {
            window.location.href = base_url;
        }
        // Others then 401 and 304 redirect to defaultRedirect url
        else if (statusCode != 304) {
            window.location.href = defaultRedirect;
        }
    }

    // Check and takecare of playerboards updates
    function UpdateBoards(boards, playerId) {
        // Loop-through playerboards
        $.each(boards, function (key, updateBoard) {
            // Get board html-element by data attribute playerid
            var board = $('.playerboard[data-playerid="' + key + '"]');

            // Loop-through response squares
            $.each(updateBoard, function (key, updateSquare) {
                // Get square html-element by data attributes x and y
                var square = board.find('.square[data-x="' + updateSquare.x + '"][data-y="' + updateSquare.y + '"]');

                /// Check if square is register hit
                if (!square.hasClass("hit") && !square.hasClass("boatHit")) {
                    // Check if any hit has been made since last update, if it has set class to html-element
                    if (updateSquare.boathit) {
                        square.addClass("boatHit");
                    }
                    else if(updateSquare.hit) {
                        square.addClass("hit");
                    }
                }
            });
        });
    }

    // Insert alert message, if player made something wrong
    function InsertAlert(messages, messageType) {
        var alertDiv;
        // Check if got any #alert-<messageType> element since before, otherwish make one 
        if ($("#alert-" + messageType).length == 0) {
            alertDiv = $("<div>").addClass("alert alert-" + messageType).attr("id", "alert-" + messageType).attr("role", "alert");
            $(alertDiv).insertAfter("#instructionsArea");
        }
        else {
            alertDiv = $("#alert-" + messageType);
        }
        
        alertDiv.html(""); // Empty element

        // Add a <ul> element to alert element
        var ul = $("<ul>");
        alertDiv.append(ul);

        // Loop-through messages, and add them to <li> elements, which we add to <ul> element
        for (var i = 0; i < messages.length; i++) {
            var li = $("<li>").html(messages[i]);
            ul.append(li);
        }
    }
});