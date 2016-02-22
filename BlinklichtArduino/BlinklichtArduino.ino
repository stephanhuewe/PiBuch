void setup()
{
	pinMode(GPIO5, OUTPUT);
}

void loop()
{
	digitalWrite(GPIO5, LOW);
	delay(500);
	digitalWrite(GPIO5, HIGH);
	delay(500);
}
