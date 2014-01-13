//a simple structure for a Point with the x - axis and y-axis coordonates
function Point(x, y) {
    this.xCoord = x;
    this.yCoord = y;
    this.isEqual = function (x, y) {
        if (this.xCoord == x && this.yCoord == y)
            return true;
        return false;
    }
}
//a class that hold the logic for the piece that we are moving
function CheckersPiece(img) {
    this.movePaths = new Array();
    this.img = img;
    this.moveIndex = 0;
    this.currentPos = new Point(0, 0);
    //"directions" keeps track of the immediate  the piece may reach.
    this.directions = new Array();
    this.addMovePath = function (movepath) {
        if (this.movePaths.length == 0) {
            this.currentPos = new Point(movepath[0].xCoord, movepath[0].yCoord);
        }

        this.movePaths.push(movepath);
        this.directions.push(movepath[1]);
    }
    this.refreshDirections = function () {
        this.resetDirections();
        for (index = 0; index < this.movePaths.length; index++) {
            this.directions.push(movePaths[index][this.moveIndex]);
        }
    }
    this.drawDirections = function () {
        for (index = 0; index < this.directions.length; index++) {
            var image = document.getElementById("img" + this.directions[index].xCoord.toString() + this.directions[index].yCoord.toString());
            if (image == null)
                alert("Couldn't find the direction image");
            image.src = "Content/Images/PossibleMove.png";
            image.onclick = function (evtArg) {
                if (!evtArg) var evtArg = window.event;
                var targetID = evtArg.target.id.toString();
                currentPiece.moveToNextPos(targetID[targetID.length - 2], targetID[targetID.length - 1]);
            }
            //TO DO: Add on click event.
        }
    }
    this.resetDirections = function () {
        var item = this.directions.pop();
        while (item != null) {
            var image = document.getElementById("img" + item.xCoord.toString() + item.yCoord.toString());
            if (image == null)
                alert("Couldn't find the direction to delete");
            image.src = "Content/Images/BlackSquare.png";
            image.onclick = null;
            item = this.directions.pop();
        }
    }
    this.clearMovePaths = function () {
        var item = this.movePaths.pop();
        while (item != null) {
            item = this.movePaths.pop();
        }
    }
    this.moveToNextPos = function (row, col) {
        this.moveIndex++;
        var validMovePaths = new Array();
        for (index = 0; index < this.movePaths.length; index++) {
            if (this.movePaths[index].length > this.moveIndex) {
                if (this.movePaths[index][this.moveIndex].isEqual(row, col))
                    validMovePaths.push(this.movePaths[index]);
            }
        }
        this.movePaths = validMovePaths;
        if (this.movePaths.length == 1) {
            this.movePiece();
            this.doPostback();
        }
        else {
            //TO DO: May be an error here?
            this.movePiece();
            this.refreshDirections();
            this.drawDirections();
        }

    }
    this.doPostback = function () {
        var playerMove = document.getElementById("ctl00_MainContent_playerMove");
        if (playerMove == null) {
            alert("The label was not found");
            return;
        }
        var stringBuilder = new String();
        for (index = 0; index < this.movePaths[0].length; index++) {
            stringBuilder += this.movePaths[0][index].xCoord + " "
            stringBuilder += this.movePaths[0][index].yCoord + " ";
        }

        playerMove.value = stringBuilder.toString();
        var submitButton = document.getElementById('ctl00_MainContent_Button1');
        submitButton.click();
        currentPiece = null;
        
    }
    this.movePiece = function () {
        var currImg = this.img;

        this.currentPos = this.movePaths[0][this.moveIndex];
        if (currImg != null) {
            this.img = document.getElementById("img" + this.currentPos.xCoord.toString() + this.currentPos.yCoord.toString());
            if (this.img == null)
                alert("The new image is not found");
            this.img.src = currImg.src;
            currImg.src = "Content/Images/BlackSquare.png";
            currImg.onclick = null;

        }
    }
}

//Procedural programming after this comment
var currentPiece = null;
function processPath(value) {

    var pathParts = value.split(' ');
    var fullMovePath = new Array();
    var index = 1;
    while (index < pathParts.length) {
        var nextPoint = new Point(pathParts[index - 1], pathParts[index]);
        fullMovePath.push(nextPoint);
        index += 2;
    }
    return fullMovePath;
}
function initializePiece(id) {
    currentPiece = new CheckersPiece(document.getElementById(id));
    if (currentPiece == null)
        alert("Something wrong with getting the movePiece");

    var stringId = id.toString();
    var baseInputId = 'Text' + stringId[stringId.length - 2] + stringId[stringId.length - 1];
    var counter = 0;
    var currentItem = document.getElementById(baseInputId + counter);
    while (currentItem != null) {
        currentPiece.addMovePath(processPath(currentItem.value));
        counter++;
        currentItem = document.getElementById(baseInputId + counter);
    }
    currentPiece.drawDirections();
}
function doClick(id) {
    //if we didn't select any piece to be moved yet
    if (currentPiece == null) {
        initializePiece(id);

    }
    //if we didn't start a move we can still pick another 
    //piece to be moved.
    if (currentPiece.moveIndex == 0) {
        currentPiece.resetDirections();
        initializePiece(id);
    }

}
          