#include "Wire.h"
#include "I2Cdev.h"
#include "MPU6050.h"
#include <Kalman.h>

Kalman kalmanX , kalmanY, kalmanZ;
MPU6050 accelgyro;
int16_t ax, ay, az, gx, gy, gz;
double accX, accY, accZ, gyroX, gyroY, gyroZ;
uint32_t timer;
double pitch, roll, yaw, AngleX, AngleY, AngleZ, Xrate, Yrate, Zrate;

char mode = 'M';

int btn1 = 13, btn2 = 12, btn3 = 11, btn4 = 10, btn0 = 9, btn = 7;
int xposPin = A0, yposPin = A1, volumePin = A2;
float volume, prev_volume;

void setup() {
  Wire.begin();
  Serial.begin(115200);
  accelgyro.initialize();
  accelgyro.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
  accX = ax; accY = ay; accZ = az; gyroX = gx; gyroY = gy; gyroZ = gz;
  pitch = atan(-accX / sqrt(accY * accY + accZ * accZ)) * RAD_TO_DEG;
   roll = atan2(accY  , accZ) * RAD_TO_DEG;
   yaw = -atan2(accX  , accY) * RAD_TO_DEG;
  kalmanY.setAngle(pitch);
  kalmanX.setAngle(roll);
  kalmanZ.setAngle(yaw);

  pinMode(btn,INPUT);
  digitalWrite(btn,HIGH);
}

void loop() {
  mode = Serial.read();
//  mode = 'C';
  if(mode == 'M'){
    //Volume
    volume = analogRead(volumePin)/1023.0; 
  
    //position
    int xp = analogRead(xposPin);
    int yp = analogRead(yposPin);
    
    //exit
    if(digitalRead(btn4)){
      Serial.print("Exit: ");
      Serial.println(1);
    }
    //btn
    else if(digitalRead(btn) == LOW){
      Serial.print("Button: ");
      Serial.println(1);
    }
    //space
    else if(digitalRead(btn3)){
      Serial.print("Space: ");
      Serial.println(1);
    }
    //move
    else if((xp < 450 || xp > 650) || (yp < 450 || yp > 650)){
      Serial.print("Movement: ");
      Serial.print(analogRead(xposPin));
      Serial.print(" ");
      Serial.println(analogRead(yposPin));
    }
    //clockwise
    else if(digitalRead(btn2)){
      Serial.print("Clockwise: ");
      Serial.println(1);
    }
    //counterclockwise
    else if(digitalRead(btn1)){
      Serial.print("Counterclockwise: ");
      Serial.println(1); 
    }
    //volume
    else if(volume >= prev_volume && volume - prev_volume >= 0.01 || volume <= prev_volume && prev_volume - volume >= 0.01){
      Serial.print("Volume: ");
      Serial.println(volume, 2);
    }else{
      Serial.println("null");
    }
    delay(10);
    prev_volume = volume;
  }else if(mode == 'R'){
    if(digitalRead(btn4)){
      Serial.print("Exit:");
      Serial.println(1);
    }else{
      //Angle
      accelgyro.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
      accX = ax; 
      accY = ay; 
      accZ = az; 
      gyroX = gx; 
      gyroY = gy; 
      gyroZ = gz;
      double dt = (double)(micros() - timer) / 1000000; // Calculate delta time
      timer = micros();
      pitch = atan(-accX / sqrt(accY * accY + accZ * accZ)) * RAD_TO_DEG;
      roll = atan2(accY,accZ) * RAD_TO_DEG;
      yaw = -atan2(accX  , accY) * RAD_TO_DEG;
      Yrate = gyroY / 131.0; // Convert to deg/s
      Xrate = gyroX / 131.0;
      Zrate = gyroZ / 131.0;
      AngleY = kalmanY.getAngle(pitch, Yrate, dt);
      AngleX = kalmanX.getAngle(roll, Xrate, dt);
      AngleZ = kalmanZ.getAngle(yaw, Zrate, dt);
    Serial.print("Angle:");
    Serial.print(AngleY);
    Serial.print(' ');
    Serial.print(AngleY);
    Serial.print(' ');
    Serial.println(-AngleY);
    } 
  }else if(mode == 'H'){
    int xp = analogRead(xposPin);
    int yp = analogRead(yposPin);
    //exit
    if(digitalRead(btn4)){
      Serial.print("Exit: ");
      Serial.println(1);
    }
    //zoom in
    else if(digitalRead(btn1)){
      Serial.print("ZoomIn: ");
      Serial.println(1);
    }
    //zoom out
    else if(digitalRead(btn2)){
      Serial.print("ZoomOut: ");
      Serial.println(1);
    }
    //space
    else if(digitalRead(btn3)){
      Serial.print("Space: ");
      Serial.println(1);
    }
    //move
    else if((xp < 450 || xp > 650) || (yp < 450 || yp > 650)){
      if(digitalRead(btn0)){
        Serial.print("CameraMovement: ");
        Serial.print(analogRead(xposPin));
        Serial.print(" ");
        Serial.println(analogRead(yposPin));
      }else{
        Serial.print("Movement: ");
        Serial.print(analogRead(xposPin));
        Serial.print(" ");
        Serial.println(analogRead(yposPin));
      }    
    }else{
      Serial.println("null");
    }
  }else if(mode == 'C'){
    int xp = analogRead(xposPin);
    int yp = analogRead(yposPin);
    //exit
    if(digitalRead(btn4)){
      Serial.print("Exit:");
      Serial.println(1);
    }else if((xp < 450 || xp > 650) || (yp < 450 || yp > 650)){
      if(digitalRead(btn0)){
        Serial.print("CameraMovement:");
        Serial.print(analogRead(xposPin));
        Serial.print(" ");
        Serial.println(analogRead(yposPin));
      }else{
        Serial.print("Movement:");
        Serial.print(analogRead(xposPin));
        Serial.print(" ");
        Serial.println(analogRead(yposPin));
      }  
    }else{
      accelgyro.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
      accX = ax; 
      accY = ay; 
      accZ = az; 
      gyroX = gx; 
      gyroY = gy; 
      gyroZ = gz;
      double dt = (double)(micros() - timer) / 1000000; // Calculate delta time
      timer = micros();
      pitch = atan(-accX / sqrt(accY * accY + accZ * accZ)) * RAD_TO_DEG;
      roll = atan2(accY,accZ) * RAD_TO_DEG;
      yaw = -atan2(accX  , accY) * RAD_TO_DEG;
      Yrate = gyroY / 131.0; // Convert to deg/s
      Xrate = gyroX / 131.0;
      Zrate = gyroZ / 131.0;
      AngleY = kalmanY.getAngle(pitch, Yrate, dt);
      AngleX = kalmanX.getAngle(roll, Xrate, dt);
      AngleZ = kalmanZ.getAngle(yaw, Zrate, dt);
    //  Serial.println(AngleX); 
    //
    //  Serial.print(',');
    //
    //  Serial.println(AngleY);
    
      Serial.print("Angle:");
    //
      Serial.print(AngleX);
      Serial.print(' ');
      Serial.print(AngleY);
      Serial.print(' ');
      Serial.println(AngleZ);
    }
    
  }
}
