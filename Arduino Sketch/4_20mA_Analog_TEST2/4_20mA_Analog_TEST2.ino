// These constants won't change. They're used to give names to the pins used:
 // Analog input pin that the potentiometer is attached to
const int LPS = A1;
const int MPS = A2;
const int MSP = A3; 
const int C1P = A4;
const int C2P = A5;
const int FAP = A6;
const int HW = 4;
const int LW = 5;


int LPSval = 0;
int MPSval = 0;
int MSPval = 0;
int C1Pval = 0;
int C2Pval = 0;
int FAPval = 0;
int HighWater = 0;
int LowWater = 0;

// value read from the pot


void setup() {
  // initialize serial communications at 9600 bps:
  Serial.begin(9600);
  pinMode(HW, INPUT);
  pinMode(LW, INPUT);
}

void loop() {

 // read the analog in value:
  LPSval = analogRead(LPS);
  delay(2);
  MPSval = analogRead(MPS);
  delay(2);
  MSPval = analogRead(MSP);
  delay(2);
  C1Pval = analogRead(C1P);
  delay(2);
  C2Pval = analogRead(C2P);
  delay(2);
  FAPval = analogRead(FAP);
  delay(2);
HighWater = digitalRead(HW);
LowWater = digitalRead(LW);

  
 


  Serial.print(LPSval);
  Serial.print(",");
  Serial.print(MPSval);
  Serial.print(",");
  Serial.print(MSPval);
  Serial.print(",");
  Serial.print(C1Pval);
  Serial.print(",");
  Serial.print(C2Pval);
  Serial.print(",");
  Serial.print(FAPval);
  Serial.print(",");
  Serial.print(HighWater);
  Serial.print(",");
  Serial.print(LowWater);
  Serial.print("*");
  

delay(10);
}
