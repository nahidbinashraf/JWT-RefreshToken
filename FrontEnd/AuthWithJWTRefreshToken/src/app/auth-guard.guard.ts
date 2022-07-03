import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardGuard implements CanActivate {

  constructor(private _router : Router){}
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      const access_token = localStorage.getItem('access_token');
      const refresh_token =  localStorage.getItem('refresh_token');
      console.log(access_token);
      if(refresh_token == null || access_token == null){
        if(state.url.indexOf("/auth") == -1){   //here -1 is not match , 0 is match 

          this._router.navigate(["/auth"]);
          return false;
         }
         else{
          return true;
         }
      }
      else {
        if(state.url.indexOf("/auth") !=-1 ){
          this._router.navigate(['/']);
          return false;
        }
        return true;
      }

      
    
  }
  
}
