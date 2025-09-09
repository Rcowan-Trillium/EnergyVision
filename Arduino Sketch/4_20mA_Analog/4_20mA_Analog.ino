// These constants won't change. They're used to give names to the pins used:
 // Analog input pin that the potentiometer is attached to
const int LPS = A1;
const int MPS = A2;
const int MSP = A3; 
const int C1P = A4;
const int C2P = A5;
const int FAP = A7;

const int numReadings = 10;
int readIndex = 0;
 
int LPSval = 0;
int LPSreadings[numReadings];      // the readings from the analog input
int LPStotal = 0;                  // the running total
int LPSaverage = 0;                // the average

int MPSval = 0;
int MPSreadings[numReadings];      // the readings from the analog input
int MPStotal = 0;                  // the running total
int MPSaverage = 0;                // the average

int MSPval = 0;
int MSPreadings[numReadings];      // the readings from the analog input
int MSPtotal = 0;                  // the running total
int MSPaverage = 0;                // the average

int C1Pval = 0;
int C1Preadings[numReadings];      // the readings from the analog input
int C1Ptotal = 0;                  // the running total
int C1Paverage = 0;                // the average

int C2Pval = 0;
int C2Preadings[numReadings];      // the readings from the analog input
int C2Ptotal = 0;                  // the running total
int C2Paverage = 0;                // the average

int FAPval = 0;
int FAPreadings[numReadings];      // the readings from the analog input
int FAPtotal = 0;                  // the running total
int FAPaverage = 0;                // the average








void setup() {
  // initialize serial communications at 9600 bps:
  Serial.begin(9600);
}

void loop() {

 // read the analog in value:
  LPSval = analogRead(LPS);
  delay(3);
  MPSval = analogRead(MPS);
  delay(3);
  MSPval = analogRead(MSP);
  delay(3);
  C1Pval = analogRead(C1P);
  delay(3);
  C2Pval = analogRead(C2P);
  delay(3);
  FAPval = analogRead(FAP);
  delay(3);
 
  
 
LPStotal = LPStotal - LPSreadings[readIndex];
LPSreadings[readIndex] = LPSval;                // read from the sensor:
LPStotal = LPStotal + LPSreadings[readIndex];  // add the reading to the total:
LPSaverage = LPStotal / numReadings;

MPStotal = MPStotal - MPSreadings[readIndex];
MPSreadings[readIndex] = MPSval;                // read from the sensor:
MPStotal = MPStotal + MPSreadings[readIndex];  // add the reading to the total:
MPSaverage = MPStotal / numReadings;

MSPtotal = MSPtotal - MSPreadings[readIndex];
MSPreadings[readIndex] = MSPval;                // read from the sensor:
MSPtotal = MSPtotal + MSPreadings[readIndex];  // add the reading to the total:
MSPaverage = MSPtotal / numReadings;

C1Ptotal = C1Ptotal - C1Preadings[readIndex];
C1Preadings[readIndex] = C1Pval;                // read from the sensor:
C1Ptotal = C1Ptotal + C1Preadings[readIndex];  // add the reading to the total:
C1Paverage = C1Ptotal / numReadings;

C2Ptotal = C2Ptotal - C2Preadings[readIndex];
C2Preadings[readIndex] = C2Pval;                // read from the sensor:
C2Ptotal = C2Ptotal + C2Preadings[readIndex];  // add the reading to the total:
C2Paverage = C2Ptotal / numReadings;

FAPtotal = FAPtotal - FAPreadings[readIndex];
FAPreadings[readIndex] = FAPval;                // read from the sensor:
FAPtotal = FAPtotal + FAPreadings[readIndex];  // add the reading to the total:
FAPaverage = FAPtotal / numReadings;






readIndex = readIndex + 1; // advance to the next position in the array:
if (readIndex >= numReadings) {
 readIndex = 0; // ...wrap around to the beginning:
}





Serial.print(LPSaverage);
Serial.print(",");
Serial.print(MPSaverage);
Serial.print(",");
Serial.print(MSPaverage);
Serial.print(",");
Serial.print(C1Paverage);
Serial.print(",");
Serial.print(C2Paverage);
Serial.print(",");
Serial.print(FAPaverage);
Serial.print("*");

delay(50);
}
