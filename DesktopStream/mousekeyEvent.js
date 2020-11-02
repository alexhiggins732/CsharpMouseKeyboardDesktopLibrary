
var LastEvent = null;
function getMouseKeyEventData(event) {
    //MouseKeyEvent(MouseKeyEventType macroEventType, EventArgs eventArgs, int timeSinceLastEvent)
    //{
    var eventType = getEventType(event.type);
    var now = new Date();
    var TimeSinceLastEvent = (LastEvent) ? now - LastEvent : 0;
    LastEvent = now;
    var eventData = 0;

    if (event.type == "keydown" || event.type == "keyup") {
        eventData = getKeyEventData(event);
    } else
    {
        eventData = getMouseEventData(eventType, event);
    }
    var word1 = (eventType & ~7) | (TimeSinceLastEvent << 3);
    return [word1, eventData];
}


function getKeyEventData(event) {
    var data = event.which;
    if (event.altKey) {
        data = data | 65536; //Shift: The SHIFT modifier key.
    }
    if (event.ctrlKey) {
        data = data | 131072; //Control: The CTRL modifier key.
    }
    if (event.shiftKey) {
        data = data | 262144; // Alt: The ALT modifier key.
    }
    return data;

}


var locationMask = 0x3FFF; // (1 << 14) - 1; 16383;
var yOffset = 14;
var buttonDataOffset = 28;
function getMouseEventData(eventType, event) {


    var xData = eventX;// -> key strokes don't capture event.pageX; // if needed & locationMask; // bits 0-13
    var yData = eventY; //key strokes dont' caputer event.pageY << yOffset; //(mouseEvent.Y & locationMask) << yOffset;// bits 14-27;
    var buttonData = eventType;
    if (eventType == 3) {
        if (event.wheelDeltaY > 0) buttonData == 6;
        else if(event.wheelDeltaY < 0) buttonData = 7;
    }
    buttonData <<= buttonDataOffset;
    var result = buttonData | yData | xData;
    return result;
}

function getEventType(eventType) {
    switch (eventType){
        case "mousemove": return 0; //"MouseMove";
        case "mousedown": return 1; "MouseDown";
        case "mouseup": return 2; "MouseUp";
        case "wheel": return 3; "MouseWheel";
        case "keydown": return 4; "KeyDown";
        case "keyup": return 5; "KeyUp";
    }
}