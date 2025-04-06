export const getUserRole = () => {
    const token = localStorage.getItem('token');
    if (!token) return null;
  
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    } catch (e) {
      console.error('Token parsing error:', e);
      return null;
    }
  };
  