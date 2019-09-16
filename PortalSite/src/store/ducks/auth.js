export const actions = {
    LOGIN: 'auth/LOGIN',
    LOGIN_SUCCESS: 'auth/LOGIN_SUCCESS',
    LOGIN_FAILED: 'auth/LOGIN_FAILED'
};

const initialState = {
    isAuthenticated: false,
    language: 'pt',
    loading: false,
    user: {
        token: null,
        name: '',
    }
}

export default function authReducer(state = initialState, action) {
    switch (action.type) {
        case actions.LOGIN:
            return state;
        default:
            return state;
    }
}