import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnInit {

  private URL = "http://localhost:5119";

  userInformation: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  jwtHelperService = new JwtHelperService();

  constructor(private _router: Router, private _http: HttpClient) {
    this.loadUserInformation();
  }

  ngOnInit(): void { }

  loadUserInformation() {
    let userData = this.userInformation.getValue();
    console.log(userData);
    if (!userData) {
      const access_token = localStorage.getItem('access_token');
      const refresh_token = localStorage.getItem('refresh_token')
      if (refresh_token && access_token) {
        const decryptJWTDecode = this.jwtHelperService.decodeToken(access_token);
        const datas = {
          access_token: access_token,
          refresh_token: refresh_token,
          name: decryptJWTDecode.Name,
          role: decryptJWTDecode.Role
        }
        this.userInformation.next(datas);
      }
    }
    else {
      console.log("Userinfor missing");
    }
  }


  loginAuth(loggedData: any): Observable<boolean> {

    return this._http.post(this.URL + "/api/Account/Login", loggedData)
      .pipe(
        map((data: any) => {
          if (!data)
            return false;
          else {
            localStorage.setItem('access_token', data.access_token);
            localStorage.setItem('refresh_token', data.refresh_token);
            const decryptJWTDecode = this.jwtHelperService.decodeToken(data.access_token);
            const datas = {
              access_token: data.access_token,
              refresh_token: data.refresh_token,
              name: decryptJWTDecode.Name,
              role: decryptJWTDecode.Role
            }
            console.log(decryptJWTDecode);
            this.userInformation.next(datas);
            return true;
          }
        })
      );
  }
}
