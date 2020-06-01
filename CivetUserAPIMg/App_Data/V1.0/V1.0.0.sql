
/* MongoDB *****************************************
use civet_hr_api_system;

db.createUser(
{
	user: "F3218466", 
	pwd: "Mongo622",
	roles: ["readWrite"]
}
);
*/
db.uapi_managers.insert(
{ "_id" : F3218466, "user_name" : "刘晓飞", "is_disabled" : false, "last_login_time" : ISODate("2017-05-04 08:37:49") }
)
db.uapi_managers.insert({
_id:'F3234119',
user_name: '奉利民',
is_disabled: false,
last_login_time: ISODate('2017-05-04 08:37:49')
})