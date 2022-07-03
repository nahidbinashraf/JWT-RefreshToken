import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/shared/auth.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{

  loginFormGroup!: FormGroup;
  constructor(private fb: FormBuilder, private authService : AuthService, private _router: Router) {  }

  ngOnInit(){
     this.loginFormGroup = this.fb.group({
      userName : ['', Validators.required],
      password : ['', Validators.required]
    })
  }

  loginSubmit(){

    this.authService.loginAuth(this.loginFormGroup.value).subscribe((value:Boolean) =>{
      if(value){
         this._router.navigate(['/dashboard']);
         alert("Login success");         
      }
      else{
        alert("Login falied");
      }
    }, err=> {
        console.log(err);
        alert("Login falied from error");
    }
    )
   // this.authService.loginAuth(this.loginFormGroup.value);
//    console.log(this.loginFormGroup.value)
  }

}
