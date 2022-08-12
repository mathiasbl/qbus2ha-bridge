let qbusName = env.get("Qbus name");
let qbusUL = env.get("Qbus UL");
let qbusLlocation = env.get("Qbus location");
let qbusCollection = global.get("QbusCollectionById");
let stateArray = [];
let msg2 = {};
Object.keys(qbusCollection).forEach((val, index) => {
    if ((qbusLlocation != "" && qbusCollection[val].location == qbusLlocation) ||
        (qbusName != "" && qbusCollection[val].name == qbusName) ||
        (qbusUL != "" && qbusCollection[val].id) == qbusUL ||
        (qbusName == "" && qbusUL == "" && qbusLlocation == "")) {
        let msg1;
        if (qbusCollection[val].type == "onoff") {
            msg1 = Switch(qbusCollection[val]);
        }
        if (qbusCollection[val].type == "analog") {
            msg1 = Light(qbusCollection[val]);
        }
        if (qbusCollection[val].type == "shutter") {
            msg1 = Cover(qbusCollection[val]);
        }
        if (qbusCollection[val].type == "thermo") {
            msg1 = Climate(qbusCollection[val]);
        }
        if (qbusCollection[val].type == "scene") {
            msg1 = Scene(qbusCollection[val]);
        }
        if (msg1 != undefined) {
            node.send(msg1);
            stateArray.push(val);
        }
    }
})

msg2.payload = stateArray;
msg2.topic = "cloudapp/QBUSMQTTGW/getState";
msg2.retain = false
return [null, msg2];

function Common(collection, type) {
    let array = {};
    let id = collection.id;
    let topic = "homeassistant/" + type + "/" + id + "/config";
    let name = "Qbus_" + collection.name.replace(/ /g, "_").replace(/\\./g, "_");
    let unique_id = name.replace(/-/g, "_");
    let command_topic = "cloudapp/QBUSMQTTGW/UL1/" + id + "/setState";
    let state_topic = "cloudapp/QBUSMQTTGW/UL1/" + id + "/state";
    array = {
        id: id,
        topic: topic,
        name: name,
        unique_id: unique_id,
        command_topic: command_topic,
        state_topic: state_topic,
    }
    return array;
}

function Switch(collection) {
    let common = Common(collection, "switch");
    let array = {};
    let payload_on = '{"id":"' + common.id + '", "type": "state", "properties": {"value": true}}';
    let payload_off = '{"id":"' + common.id + '", "type": "state", "properties": {"value": false}}';
    let value_template = "{{ value_json['properties']['value'] }}";
    array = {
        name: common.name,
        unique_id: common.unique_id,
        command_topic: common.command_topic,
        payload_on: '{"id":"' + common.id + '", "type": "state", "properties": {"value": true}}',
        payload_off: '{"id":"' + common.id + '", "type": "state", "properties": {"value": false}}',
        state_topic: common.state_topic,
        value_template: value_template,
        state_on: true,
        state_off: false,
    }
    let msg = {};
    msg.topic = common.topic;
    msg.payload = array;
    msg.retain = true;
    return msg;
}

function Light(collection) {
    let common = Common(collection, "light");
    let array = {};
    array = {
        name: common.name,
        unique_id: common.unique_id,
        schema: "template",
        brightness_template: "{{ value_json.properties.value | float | multiply(2.55) | round(0) }}",
        command_topic: common.command_topic,
        command_off_template: '{"id":"' + common.id + '", "type": "state", "properties": {"value": 0}}',
        command_on_template: '{%- if brightness is defined -%} {"id":"' + common.id + '","type":"state","properties":{"value":{{brightness | float | multiply(0.39215686) | round(0)}}}} {%- else -%} {"id":"' + common.id + '","type":"state","properties":{"value":100}} {%- endif -%} }',
        state_topic: common.state_topic,
        state_template: '{% if value_json.properties.value > 0 %} on {% else %} off {% endif %}',
    }
    let msg = {};
    msg.topic = common.topic;
    msg.payload = array;
    msg.retain = true;
    return (msg);
}

function Cover(collection) {
    let common = Common(collection, "cover");
    let array = {};
    array = {
        name: common.name,
        unique_id: common.unique_id,
        command_topic: common.command_topic,
        payload_close: '{"id":"' + common.id + '", "type": "state", "properties": {"state": "down"}}',
        payload_open: '{"id":"' + common.id + '", "type": "state", "properties": {"state": "up"}}',
        payload_stop: '{"id":"' + common.id + '", "type": "state", "properties": {"state": "stop"}}',
        state_topic: common.state_topic,
        state_closing: "down",
        state_opening: "up",
        state_stopped: "stop",
        value_template: "{{ value_json['properties']['state'] }}",
        optimistic: true,
    }
    let msg = {};
    msg.topic = common.topic;
    msg.payload = array;
    msg.retain = true;
    return (msg);
}

function Climate(collection) {
    let common = Common(collection, "climate");
    let array = {};
    array = {
        name: common.name,
        unique_id: common.unique_id,
        current_temperature_topic: common.state_topic,
        current_temperature_template: "{{ value_json['properties']['currTemp'] }}",
        modes: ["heat"],
        precision: 0.5,
        preset_mode_command_topic: common.command_topic,
        preset_mode_command_template: '{"id":"' + common.id + '", "type": "state", "properties": {"currRegime": "{{ value }}" }}',
        preset_mode_state_topic: common.state_topic,
        preset_mode_value_template: "{{ value_json['properties']['currRegime'] }}",
        preset_modes: ["MANUEEL", "VORST", "ECONOMY", "COMFORT", "NACHT"],
        temperature_command_topic: common.command_topic,
        temperature_command_template: '{"id":"' + common.id + '", "type": "state", "properties": {"setTemp": {{ value }}}}',
        temperature_state_topic: common.state_topic,
        temperature_state_template: "{{ value_json['properties']['setTemp'] }}",
        temperature_unit: "C",
        temp_step: 0.5,
    }
    let msg = {};
    msg.topic = common.topic;
    msg.payload = array;
    msg.retain = true;
    return (msg);
}

function Scene(collection) {
    let common = Common(collection, "scene");
    let array = {};
    array = {
        name: common.name,
        unique_id: common.unique_id,
        command_topic: common.command_topic,
        payload_on: '{"id":"' + common.id + '","type":"action","action":"active","properties":{}}',
    }
    let msg = {};
    msg.topic = common.topic;
    msg.payload = array;
    msg.retain = true;
    return msg;
}

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}